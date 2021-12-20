using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

    public ObjectMapperManager()
    {
        _instanceCount = Interlocked.Increment(ref _instCount);
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

    private readonly Dictionary<MapperKey, int> _objectsMapperIds = new();

    private readonly List<ObjectsMapperDescr> _objectsMappersList = new();

    internal ObjectsMapperDescr GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        lock (this)
        {
            to ??= typeof(object);
            from ??= typeof(object);

            var mapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName());

            if (!_objectsMapperIds.TryGetValue(mapperTypeKey, out var mapperId))
            {
                Console.WriteLine($"new mapper ID:{mapperId},key:{mapperTypeKey}");
                var result = new ObjectsMapperDescr(null, mapperTypeKey, 0);
                AddMapper(result);

                var mapperTypeName = GetMapperTypeName(from, to);
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

            return _objectsMappersList[mapperId];
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

        var result = (ObjectsMapperBaseImpl)Activator.CreateInstance(typeBuilder.CreateType());
        result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.StoredObjects.ToArray());
        return result;
    }

    private ObjectsMapperDescr GetMapperByKey(MapperKey key)
    {
        return _objectsMappersList[_objectsMapperIds[key]];
    }

    private int AddMapper(ObjectsMapperDescr descr)
    {
        descr.Id = _objectsMappersList.Count;
        _objectsMappersList.Add(descr);
        _objectsMapperIds.Add(descr.Key, descr.Id);
        return descr.Id;
    }

    private int GetNextMapperId()
    {
        return _objectsMapperIds.Count;
    }

    private bool IsMapperCreated(MapperKey key)
    {
        return _objectsMapperIds.ContainsKey(key);
    }

    private string GetMapperTypeKey(Type from, Type to, string mapperName)
    {
        return GetMapperTypeName(from, to) + (mapperName ?? "");
    }

    private string GetMapperTypeName(Type from, Type to)
    {
        var fromFullName = from == null ? "null" : from.FullName;
        var toFullName = to == null ? "null" : to.FullName;
        return "ObjectsMapper" + _instanceCount + "_" + fromFullName + "_" + toFullName;
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

    private readonly string _mapperName;

    private readonly Type _typeFrom;

    private readonly Type _typeTo;

    public MapperKey(Type typeFrom, Type typeTo, string mapperName)
    {
        _typeFrom = typeFrom;
        _typeTo = typeTo;
        _mapperName = mapperName;

        _hash = HashCode.Combine(typeFrom, typeTo, mapperName);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        var rhs = (MapperKey)obj;
        return Equals(rhs);
    }

    public bool Equals(MapperKey x, MapperKey y)
    {
        if (x != null)
            return x.Equals(y);
        if (y != null)
            return y.Equals(x);
        return true;
    }

    bool IEquatable<MapperKey>.Equals(MapperKey rhs)
    {
        return _hash == rhs._hash && _typeFrom == rhs._typeFrom && _typeTo == rhs._typeTo
               && _mapperName == rhs._mapperName;
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public int GetHashCode(MapperKey obj)
    {
        return obj.GetHashCode();
    }

    public override string ToString()
    {
        return $"mapperName:{_mapperName}_from:{_typeFrom.FullName}_to:{_typeTo.FullName}";
    }

}