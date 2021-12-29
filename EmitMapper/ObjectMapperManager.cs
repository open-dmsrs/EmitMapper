using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using EmitMapper.EmitBuilders;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;

namespace EmitMapper;

/// <summary>
///     Class for maintaining and generating Mappers.
/// </summary>
public class ObjectMapperManager
{
    private static readonly Lazy<ObjectMapperManager> _LazyDefaultInstance = new();

    private static int _totalInstCount;

    private readonly int _CurrentInstanceId;

    public ObjectMapperManager()
    {
        _CurrentInstanceId = Interlocked.Increment(ref _totalInstCount);
    }

    public static ObjectMapperManager DefaultInstance => _LazyDefaultInstance.Value;

    /// <summary>
    ///     Returns a Mapper instance for specified types.
    /// </summary>
    /// <typeparam name="TFrom">Type of source object</typeparam>
    /// <typeparam name="TTo">Type of destination object</typeparam>
    /// <returns></returns>
    public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>()
    {
        return new ObjectsMapper<TFrom, TTo>(GetMapperImpl(typeof(TFrom), typeof(TTo), DefaultMapConfig.Instance));
    }

    /// <summary>
    ///     Returns a Mapper instance for specified types.
    /// </summary>
    /// <typeparam name="TFrom">Type of source object</typeparam>
    /// <typeparam name="TTo">Type of destination object</typeparam>
    /// <param name="mappingConfigurator">Object which configures mapping.</param>
    /// <returns>Mapper</returns>
    public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>(IMappingConfigurator mappingConfigurator)
    {
        return new ObjectsMapper<TFrom, TTo>(GetMapperImpl(typeof(TFrom), typeof(TTo), mappingConfigurator));
    }

    /// <summary>
    ///     Returns a mapper implementation instance for specified types.
    /// </summary>
    /// <param name="from">Type of source object</param>
    /// <param name="to">Type of destination object</param>
    /// <param name="mappingConfigurator">Object which configures mapping.</param>
    /// <returns>Mapper</returns>
    public ObjectsMapperBaseImpl GetMapperImpl(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        return GetMapperInt(from, to, mappingConfigurator).Mapper;
    }

    #region Non-public members

    private static readonly Dictionary<MapperKey, ObjectsMapperDescr> _objectsMapperIds =
        new(new MapperKey(null, null, null, 0));


    internal ObjectsMapperDescr GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        to ??= typeof(object);
        from ??= typeof(object);
        var mapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName(), _CurrentInstanceId);

        ObjectsMapperDescr result;
        if (_objectsMapperIds.TryGetValue(mapperTypeKey, out result))
            return result;

        lock (_objectsMapperIds)
        {
            if (_objectsMapperIds.TryGetValue(mapperTypeKey, out result))
                return result;

            Debug.WriteLine($"new mapper ID:{_CurrentInstanceId},key:{mapperTypeKey}");
            result = new ObjectsMapperDescr(null, mapperTypeKey, 0);
            _objectsMapperIds.Add(mapperTypeKey, result);

            var mapperTypeName = mapperTypeKey.GetMapperTypeName();
            ObjectsMapperBaseImpl createdMapper;
            if (MapperPrimitiveImpl.IsSupportedType(to))
            {
                createdMapper = new MapperPrimitiveImpl(this, from, to, mappingConfigurator);
            }
            else if (MapperForCollectionImpl.IsSupportedType(to))
            {
                var mapper = GetMapperInt(
                    MapperForCollectionImpl.GetSubMapperTypeFrom(from),
                    MapperForCollectionImpl.GetSubMapperTypeTo(to),
                    mappingConfigurator);

                createdMapper = MapperForCollectionImpl.CreateInstance(
                    mapperTypeName + GetNextMapperId(),
                    this,
                    from,
                    to,
                    mapper,
                    mappingConfigurator);
            }
            else
            {
                createdMapper = BuildObjectsMapper(
                    mapperTypeName + GetNextMapperId(),
                    from,
                    to,
                    mappingConfigurator);
            }

            result.Mapper = createdMapper;
            return result;
        }
    }

    private ObjectsMapperBaseImpl BuildObjectsMapper(
        string mapperTypeName,
        Type from,
        Type to,
        IMappingConfigurator mappingConfigurator)
    {
        var typeBuilder = DynamicAssemblyManager.DefineMapperType(mapperTypeName);
        CreateTargetInstanceBuilder.BuildCreateTargetInstanceMethod(to, typeBuilder);

        var mappingBuilder = new MappingBuilder(this, from, to, typeBuilder, mappingConfigurator);
        mappingBuilder.BuildCopyImplMethod();

        var result = Expression.Lambda<Func<ObjectsMapperBaseImpl>>(
            Expression.New(typeBuilder.CreateType())).Compile()();
        result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.StoredObjects.ToArray());
        return result;
    }


    private int GetNextMapperId()
    {
        return _objectsMapperIds.Count;
    }

    #endregion
}

public class ObjectsMapperDescr
{
    public int Id;

    public MapperKey Key;

    public ObjectsMapperBaseImpl Mapper;

    public ObjectsMapperDescr(ObjectsMapperBaseImpl mapper, MapperKey key, int id)
    {
        Mapper = mapper;
        Key = key;
        Id = id;
    }
}

public class LazyConcurrentDictionary<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, Lazy<TValue>> concurrentDictionary;

    public LazyConcurrentDictionary()
    {
        concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
    }

    public int Count => concurrentDictionary.Count;

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        var lazyResult = concurrentDictionary.GetOrAdd(
            key,
            k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));
        return lazyResult.Value;
    }
}

public struct MapperKey : IEqualityComparer<MapperKey>, IEquatable<MapperKey>
{
    private readonly int _hash;
    private readonly string _mapperTypeName;

    public MapperKey(Type typeFrom, Type typeTo, string configName, int currentInstantId)
    {
        _mapperTypeName = $"M{currentInstantId}_{typeFrom?.FullName}_{typeTo?.FullName}_{configName}";
        _hash = _mapperTypeName.GetHashCode();
    }


    public bool Equals(MapperKey x, MapperKey y)
    {
        return x._mapperTypeName == y._mapperTypeName;
    }


    public int GetHashCode(MapperKey obj)
    {
        return obj._hash;
    }

    public bool Equals(MapperKey rhs)
    {
        return _mapperTypeName == rhs._mapperTypeName;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        var rhs = (MapperKey)obj;
        return _hash == rhs._hash && _mapperTypeName == rhs._mapperTypeName;
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public override string ToString()
    {
        return _mapperTypeName;
    }

    public string GetMapperTypeName()
    {
        return _mapperTypeName;
    }
}