using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    private static int _instCount;

    private readonly int _instanceCount;

    public ObjectMapperManager() { _instanceCount = Interlocked.Increment(ref _instCount); }

    public static ObjectMapperManager DefaultInstance => _LazyDefaultInstance.Value;

    /// <summary>
    ///     Returns a Mapper instance for specified types.
    /// </summary>
    /// <typeparam name="TFrom">Type of source object</typeparam>
    /// <typeparam name="TTo">Type of destination object</typeparam>
    /// <returns></returns>
    public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>() { return new ObjectsMapper<TFrom, TTo>(GetMapperImpl(typeof(TFrom), typeof(TTo), DefaultMapConfig.Instance)); }

    /// <summary>
    ///     Returns a Mapper instance for specified types.
    /// </summary>
    /// <typeparam name="TFrom">Type of source object</typeparam>
    /// <typeparam name="TTo">Type of destination object</typeparam>
    /// <param name="mappingConfigurator">Object which configures mapping.</param>
    /// <returns>Mapper</returns>
    public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>(IMappingConfigurator mappingConfigurator) { return new ObjectsMapper<TFrom, TTo>(GetMapperImpl(typeof(TFrom), typeof(TTo), mappingConfigurator)); }

    /// <summary>
    ///     Returns a mapper implementation instance for specified types.
    /// </summary>
    /// <param name="from">Type of source object</param>
    /// <param name="to">Type of destination object</param>
    /// <param name="mappingConfigurator">Object which configures mapping.</param>
    /// <returns>Mapper</returns>
    public ObjectsMapperBaseImpl GetMapperImpl(Type from, Type to, IMappingConfigurator mappingConfigurator) { return GetMapperInt(from, to, mappingConfigurator).Mapper; }

    #region Non-public members

    private static readonly ConcurrentDictionary<MapperKey, ObjectsMapperDescr> _objectsMappers = new();


    internal ObjectsMapperDescr GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        lock (this)
        {
            to ??= typeof(object);
            from ??= typeof(object);

            var mapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName());

            if (!_objectsMappers.TryGetValue(mapperTypeKey, out var mapperDescr))
            {
                Console.WriteLine($"new mapper key:{mapperTypeKey}");
                var result = new ObjectsMapperDescr(null, mapperTypeKey, 0);

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
                        mappingConfigurator
                    );

                    createdMapper = MapperForCollectionImpl.CreateInstance(
                        mapperTypeName,
                        this,
                        from,
                        to,
                        mapper,
                        mappingConfigurator
                    );
                }
                else
                {
                    createdMapper = BuildObjectsMapper(
                        mapperTypeName,
                        from,
                        to,
                        mappingConfigurator
                    );
                }

                result.Mapper = createdMapper;
                if (_objectsMappers.TryAdd(mapperTypeKey, result))
                    return result;
                throw new EmitMapperException("重复的Key，无法加入缓存");
            }

            return mapperDescr;
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

        var result = Expression.Lambda<Func<ObjectsMapperBaseImpl>>(Expression.New(typeBuilder.CreateType()))
            .Compile()
            .Invoke();
        result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.StoredObjects.ToArray());
        return result;
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

public class MapperKey : IEqualityComparer<MapperKey>, IEquatable<MapperKey>
{
    private readonly int _hash;

    private readonly string _configName;

    private readonly Type _typeFrom;

    private readonly Type _typeTo;

    public MapperKey(Type typeFrom, Type typeTo, string configName)
    {
        _typeFrom = typeFrom;
        _typeTo = typeTo;
        _configName = configName;

        _hash = HashCode.Combine(typeFrom, typeTo, configName);
    }

    public bool Equals(MapperKey x, MapperKey y)
    {
        if (x != null)
            return x.Equals(y);
        if (y != null)
            return y.Equals(x);
        return true;
    }

    public int GetHashCode(MapperKey obj) { return obj.GetHashCode(); }

    bool IEquatable<MapperKey>.Equals(MapperKey rhs) { return _hash == rhs._hash && _typeFrom == rhs._typeFrom && _typeTo == rhs._typeTo && _configName == rhs._configName; }

    public string GetMapperTypeName() { return ToString(); }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        var rhs = (MapperKey)obj;
        return Equals(rhs);
    }

    public override int GetHashCode() { return _hash; }

    public override string ToString() { return $"F_{_typeFrom?.FullName ?? "null"}_T_{_typeTo?.FullName ?? "null"}_{_configName ?? "null"}"; }
}