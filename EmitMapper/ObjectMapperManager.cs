namespace EmitMapper;

using System;
using System.Collections.Generic;
using System.Threading;

using EmitMapper.EmitBuilders;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;

/// <summary>
///     Class for maintaining and generating Mappers.
/// </summary>
public class ObjectMapperManager
{
    private static readonly Lazy<ObjectMapperManager> _LazyDefaultInstance = new();

    private static int __InstanceCount;

    private readonly int _instanceCount;

    public ObjectMapperManager()
    {
        Interlocked.Increment(ref __InstanceCount);
        this._instanceCount = __InstanceCount;
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
        return new ObjectsMapper<TFrom, TTo>(this.GetMapperImpl(typeof(TFrom), typeof(TTo), DefaultMapConfig.Instance));
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
        return new ObjectsMapper<TFrom, TTo>(this.GetMapperImpl(typeof(TFrom), typeof(TTo), mappingConfigurator));
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
        return this.GetMapperInt(from, to, mappingConfigurator).Mapper;
    }

    #region Non-public members

    private readonly Dictionary<MapperKey, int> _objectsMapperIds = new();

    private readonly List<ObjectsMapperDescr> _objectsMappersList = new();

    internal ObjectsMapperDescr GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        lock (this)
        {
            to ??= typeof(object);
            @from ??= typeof(object);

            var mapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName());

            if (!this._objectsMapperIds.TryGetValue(mapperTypeKey, out var mapperId))
            {
                var result = new ObjectsMapperDescr(null, mapperTypeKey, 0);
                this.AddMapper(result);

                var mapperTypeName = this.GetMapperTypeName(from, to);
                ObjectsMapperBaseImpl createdMapper;
                if (MapperPrimitiveImpl.IsSupportedType(to))
                {
                    createdMapper = new MapperPrimitiveImpl(this, from, to, mappingConfigurator);
                }
                else if (MapperForCollectionImpl.IsSupportedType(to))
                {
                    var mapper = this.GetMapperInt(
                        MapperForCollectionImpl.GetSubMapperTypeFrom(from),
                        MapperForCollectionImpl.GetSubMapperTypeTo(to),
                        mappingConfigurator);

                    createdMapper = MapperForCollectionImpl.CreateInstance(
                        mapperTypeName + this.GetNextMapperId(),
                        this,
                        from,
                        to,
                        mapper,
                        mappingConfigurator);
                }
                else
                {
                    createdMapper = this.BuildObjectsMapper(
                        mapperTypeName + this.GetNextMapperId(),
                        from,
                        to,
                        mappingConfigurator);
                }

                result.Mapper = createdMapper;
                return result;
            }

            return this._objectsMappersList[mapperId];
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
        return this._objectsMappersList[this._objectsMapperIds[key]];
    }

    private int AddMapper(ObjectsMapperDescr descr)
    {
        descr.Id = this._objectsMappersList.Count;
        this._objectsMappersList.Add(descr);
        this._objectsMapperIds.Add(descr.Key, descr.Id);
        return descr.Id;
    }

    private int GetNextMapperId()
    {
        return this._objectsMapperIds.Count;
    }

    private bool IsMapperCreated(MapperKey key)
    {
        return this._objectsMapperIds.ContainsKey(key);
    }

    private string GetMapperTypeKey(Type from, Type to, string mapperName)
    {
        return this.GetMapperTypeName(from, to) + (mapperName ?? "");
    }

    private string GetMapperTypeName(Type from, Type to)
    {
        var fromFullName = from == null ? "null" : from.FullName;
        var toFullName = to == null ? "null" : to.FullName;
        return "ObjectsMapper" + this._instanceCount + "_" + fromFullName + "_" + toFullName;
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
        this.Mapper = mapper;
        this.Key = key;
        this.Id = id;
    }
}

public class MapperKey
{
    private readonly int _hash;

    private readonly string _mapperName;

    private readonly Type _typeFrom;

    private readonly Type _typeTo;

    public MapperKey(Type typeFrom, Type typeTo, string mapperName)
    {
        this._typeFrom = typeFrom;
        this._typeTo = typeTo;
        this._mapperName = mapperName;
        this._hash = typeFrom.GetHashCode() + typeTo.GetHashCode()
                                            + (mapperName == null ? 0 : mapperName.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        var rhs = (MapperKey)obj;
        return this._hash == rhs._hash && this._typeFrom == rhs._typeFrom && this._typeTo == rhs._typeTo
               && this._mapperName == rhs._mapperName;
    }

    public override int GetHashCode()
    {
        return this._hash;
    }
}