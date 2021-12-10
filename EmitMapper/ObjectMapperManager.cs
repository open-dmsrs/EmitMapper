namespace EmitMapper;

using System;
using System.Collections.Generic;

using EmitMapper.EmitBuilders;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;

/// <summary>
///     Class for maintaining and generating Mappers.
/// </summary>
public class ObjectMapperManager
{
    public static ObjectMapperManager _defaultInstance;

    public ObjectMapperManager()
    {
        lock (typeof(ObjectMapperManager))
        {
            _instanceCount++;
            this.instanceCount = _instanceCount;
        }
    }

    public static ObjectMapperManager DefaultInstance
    {
        get
        {
            if (_defaultInstance == null)
                lock (typeof(ObjectMapperManager))
                {
                    if (_defaultInstance == null)
                        _defaultInstance = new ObjectMapperManager();
                }

            return _defaultInstance;
        }
    }

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
        return this.GetMapperInt(from, to, mappingConfigurator).mapper;
    }

    #region Non-public members

    private static int _instanceCount;

    private readonly int instanceCount;

    private readonly Dictionary<MapperKey, int> objectsMapperIds = new();

    private readonly List<ObjectsMapperDescr> objectsMappersList = new();

    internal ObjectsMapperDescr GetMapperInt(Type from, Type to, IMappingConfigurator mappingConfigurator)
    {
        lock (this)
        {
            if (to == null)
                to = typeof(object);
            if (from == null)
                from = typeof(object);

            var MapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName());
            ObjectsMapperDescr result;

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                            int mapperId;
                            if (!objectsMapperIds.TryGetValue(MapperTypeKey, out mapperId))
            After:
                            int mapperId;
                            if (!objectsMapperIds.TryGetValue(MapperTypeKey, out mapperId))
            */
            if (!this.objectsMapperIds.TryGetValue(MapperTypeKey, out var mapperId))
            {
                result = new ObjectsMapperDescr(null, MapperTypeKey, 0);
                this.AddMapper(result);

                var MapperTypeName = this.GetMapperTypeName(from, to);
                ObjectsMapperBaseImpl createdMapper;
                if (MapperPrimitiveImpl.IsSupportedType(to))
                {
                    createdMapper = new MapperPrimitiveImpl(this, from, to, mappingConfigurator);
                }
                else if (MapperForCollectionImpl.IsSupportedType(to))
                {
                    var Mapper = this.GetMapperInt(
                        MapperForCollectionImpl.GetSubMapperTypeFrom(from),
                        MapperForCollectionImpl.GetSubMapperTypeTo(to),
                        mappingConfigurator);

                    createdMapper = MapperForCollectionImpl.CreateInstance(
                        MapperTypeName + this.GetNextMapperId(),
                        this,
                        from,
                        to,
                        Mapper,
                        mappingConfigurator);
                }
                else
                {
                    createdMapper = this.BuildObjectsMapper(
                        MapperTypeName + this.GetNextMapperId(),
                        from,
                        to,
                        mappingConfigurator);
                }

                result.mapper = createdMapper;
                return result;
            }

            return this.objectsMappersList[mapperId];
        }
    }

    private ObjectsMapperBaseImpl BuildObjectsMapper(
        string MapperTypeName,
        Type from,
        Type to,
        IMappingConfigurator mappingConfigurator)
    {
        var typeBuilder = DynamicAssemblyManager.DefineMapperType(MapperTypeName);
        CreateTargetInstanceBuilder.BuildCreateTargetInstanceMethod(to, typeBuilder);

        var mappingBuilder = new MappingBuilder(this, from, to, typeBuilder, mappingConfigurator);
        mappingBuilder.BuildCopyImplMethod();

        var result = (ObjectsMapperBaseImpl)Activator.CreateInstance(typeBuilder.CreateType());
        result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.storedObjects.ToArray());
        return result;
    }

    private ObjectsMapperDescr GetMapperByKey(MapperKey key)
    {
        return this.objectsMappersList[this.objectsMapperIds[key]];
    }

    private int AddMapper(ObjectsMapperDescr descr)
    {
        descr.id = this.objectsMappersList.Count;
        this.objectsMappersList.Add(descr);
        this.objectsMapperIds.Add(descr.key, descr.id);
        return descr.id;
    }

    private int GetNextMapperId()
    {
        return this.objectsMapperIds.Count;
    }

    private bool IsMapperCreated(MapperKey key)
    {
        return this.objectsMapperIds.ContainsKey(key);
    }

    private string GetMapperTypeKey(Type from, Type to, string mapperName)
    {
        return this.GetMapperTypeName(from, to) + (mapperName ?? "");
    }

    private string GetMapperTypeName(Type from, Type to)
    {
        var fromFullName = from == null ? "null" : from.FullName;
        var toFullName = to == null ? "null" : to.FullName;
        return "ObjectsMapper" + this.instanceCount + "_" + fromFullName + "_" + toFullName;
    }

    #endregion
}

public class ObjectsMapperDescr
{
    public int id;

    public MapperKey key;

    public ObjectsMapperBaseImpl mapper;

    public ObjectsMapperDescr(ObjectsMapperBaseImpl mapper, MapperKey key, int id)
    {
        this.mapper = mapper;
        this.key = key;
        this.id = id;
    }
}

public class MapperKey
{
    private readonly int _hash;

    private readonly string _mapperName;

    private readonly Type _TypeFrom;

    private readonly Type _TypeTo;

    public MapperKey(Type TypeFrom, Type TypeTo, string mapperName)
    {
        this._TypeFrom = TypeFrom;
        this._TypeTo = TypeTo;
        this._mapperName = mapperName;
        this._hash = TypeFrom.GetHashCode() + TypeTo.GetHashCode()
                                            + (mapperName == null ? 0 : mapperName.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        var rhs = (MapperKey)obj;
        return this._hash == rhs._hash && this._TypeFrom == rhs._TypeFrom && this._TypeTo == rhs._TypeTo
               && this._mapperName == rhs._mapperName;
    }

    public override int GetHashCode()
    {
        return this._hash;
    }
}