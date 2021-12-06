using EmitMapper.EmitBuilders;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;
using EmitObjectMapper.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EmitMapper
{
    /// <summary>
    /// Class for maintaining and generating Mappers.
    /// </summary>
    public class ObjectMapperManager
    {
        public static ObjectMapperManager _defaultInstance = null;

        public static ObjectMapperManager DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (typeof(ObjectMapperManager))
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new ObjectMapperManager();
                        }
                    }
                }
                return _defaultInstance;
            }
        }

        public ObjectMapperManager()
        {
            lock (typeof(ObjectMapperManager))
            {
                _instanceCount++;
                instanceCount = _instanceCount;
            }
        }

        /// <summary>
        /// Returns a Mapper instance for specified types.
        /// </summary>
        /// <typeparam name="TFrom">Type of source object</typeparam>
        /// <typeparam name="TTo">Type of destination object</typeparam>
        /// <returns></returns>
        public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>()
        {
            return new ObjectsMapper<TFrom, TTo>(
                GetMapperImpl(
                    typeof(TFrom),
                    typeof(TTo),
                    DefaultMapConfig.Instance
                )
            );
        }

        /// <summary>
        /// Returns a Mapper instance for specified types.
        /// </summary>
        /// <typeparam name="TFrom">Type of source object</typeparam>
        /// <typeparam name="TTo">Type of destination object</typeparam>
        /// <param name="mappingConfigurator">Object which configures mapping.</param>
        /// <returns>Mapper</returns>
        public ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>(IMappingConfigurator mappingConfigurator)
        {
            return new ObjectsMapper<TFrom, TTo>(
                GetMapperImpl(
                    typeof(TFrom),
                    typeof(TTo),
                    mappingConfigurator
                    )
                );
        }

        /// <summary>
        /// Returns a mapper implementation instance for specified types.
        /// </summary>
        /// <param name="from">Type of source object</param>
        /// <param name="to">Type of destination object</param>
        /// <param name="mappingConfigurator">Object which configures mapping.</param>
        /// <returns>Mapper</returns>
        public ObjectsMapperBaseImpl GetMapperImpl(
            Type from,
            Type to,
            IMappingConfigurator mappingConfigurator)
        {
            return GetMapperInt(from, to, mappingConfigurator).mapper;
        }

        #region Non-public members

        private static int _instanceCount = 0;
        private readonly int instanceCount = 0;

        private readonly Dictionary<MapperKey, int> objectsMapperIds = new Dictionary<MapperKey, int>();
        private readonly List<ObjectsMapperDescr> objectsMappersList = new List<ObjectsMapperDescr>();

        internal ObjectsMapperDescr GetMapperInt(
            Type from,
            Type to,
            IMappingConfigurator mappingConfigurator)
        {
            lock (this)
            {
                if (to == null)
                {
                    to = typeof(object);
                }
                if (from == null)
                {
                    from = typeof(object);
                }


                MapperKey MapperTypeKey = new MapperKey(from, to, mappingConfigurator.GetConfigurationName());
                ObjectsMapperDescr result;


                /* Unmerged change from project 'EmitMapper (netstandard2.1)'
                Before:
                                int mapperId;
                                if (!objectsMapperIds.TryGetValue(MapperTypeKey, out mapperId))
                After:
                                int mapperId;
                                if (!objectsMapperIds.TryGetValue(MapperTypeKey, out mapperId))
                */
                if (!objectsMapperIds.TryGetValue(MapperTypeKey, out int mapperId))
                {
                    result = new ObjectsMapperDescr(
                        null,
                        MapperTypeKey,
                        0
                    );
                    AddMapper(result);

                    string MapperTypeName = GetMapperTypeName(from, to);
                    ObjectsMapperBaseImpl createdMapper;
                    if (MapperPrimitiveImpl.IsSupportedType(to))
                    {
                        createdMapper = new MapperPrimitiveImpl(this, from, to, mappingConfigurator);
                    }
                    else if (MapperForCollectionImpl.IsSupportedType(to))
                    {
                        ObjectsMapperDescr Mapper = GetMapperInt(
                            MapperForCollectionImpl.GetSubMapperTypeFrom(from),
                            MapperForCollectionImpl.GetSubMapperTypeTo(to),
                            mappingConfigurator);

                        createdMapper = MapperForCollectionImpl.CreateInstance(
                            MapperTypeName + GetNextMapperId(),
                            this,
                            from,
                            to,
                            Mapper,
                            mappingConfigurator
                            );
                    }
                    else
                    {
                        createdMapper = BuildObjectsMapper(
                            MapperTypeName + GetNextMapperId(),
                            from,
                            to,
                            mappingConfigurator
                            );
                    }

                    result.mapper = createdMapper;
                    return result;
                }
                else
                {
                    return objectsMappersList[mapperId];
                }
            }
        }

        private ObjectsMapperBaseImpl BuildObjectsMapper(
            string MapperTypeName,
            Type from,
            Type to,
            IMappingConfigurator mappingConfigurator)
        {
            TypeBuilder typeBuilder = DynamicAssemblyManager.DefineMapperType(MapperTypeName);
            CreateTargetInstanceBuilder.BuildCreateTargetInstanceMethod(to, typeBuilder);


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var mappingBuilder = new MappingBuilder(
            After:
                        var mappingBuilder = new MappingBuilder(
            */
            MappingBuilder mappingBuilder = new MappingBuilder(
                this,
                from,
                to,
                typeBuilder,
                mappingConfigurator
            );
            mappingBuilder.BuildCopyImplMethod();

            ObjectsMapperBaseImpl result =
                (ObjectsMapperBaseImpl)Activator.CreateInstance(typeBuilder.CreateType());
            result.Initialize(this, from, to, mappingConfigurator, mappingBuilder.storedObjects.ToArray());
            return result;
        }

        private ObjectsMapperDescr GetMapperByKey(MapperKey key)
        {
            return objectsMappersList[objectsMapperIds[key]];
        }

        private int AddMapper(ObjectsMapperDescr descr)
        {
            descr.id = objectsMappersList.Count;
            objectsMappersList.Add(descr);
            objectsMapperIds.Add(descr.key, descr.id);
            return descr.id;
        }

        private int GetNextMapperId()
        {
            return objectsMapperIds.Count;
        }

        private bool IsMapperCreated(MapperKey key)
        {
            return objectsMapperIds.ContainsKey(key);
        }

        private string GetMapperTypeKey(Type from, Type to, string mapperName)
        {
            return GetMapperTypeName(from, to) + (mapperName ?? "");
        }

        private string GetMapperTypeName(Type from, Type to)
        {
            string fromFullName = from == null ? "null" : from.FullName;
            string toFullName = to == null ? "null" : to.FullName;
            return "ObjectsMapper" + instanceCount + "_" + fromFullName + "_" + toFullName;
        }

        #endregion
    }


    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class ObjectsMapperDescr
    After:
        class ObjectsMapperDescr
    */
    internal class ObjectsMapperDescr
    {
        public ObjectsMapperBaseImpl mapper;
        public MapperKey key;
        public int id;
        public ObjectsMapperDescr(ObjectsMapperBaseImpl mapper, MapperKey key, int id)
        {
            this.mapper = mapper;
            this.key = key;
            this.id = id;
        }
    }


    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class MapperKey
    After:
        class MapperKey
    */
    internal class MapperKey
    {

        /* Unmerged change from project 'EmitMapper (netstandard2.1)'
        Before:
                Type _TypeFrom;
                Type _TypeTo;
                string _mapperName;
                int _hash;
        After:
                Type _TypeFrom;
                Type _TypeTo;
                string _mapperName;
                int _hash;
        */
        private readonly Type _TypeFrom;
        private readonly Type _TypeTo;
        private readonly string _mapperName;
        private readonly int _hash;

        public MapperKey(Type TypeFrom, Type TypeTo, string mapperName)
        {
            _TypeFrom = TypeFrom;
            _TypeTo = TypeTo;
            _mapperName = mapperName;
            _hash = TypeFrom.GetHashCode() + TypeTo.GetHashCode() + (mapperName == null ? 0 : mapperName.GetHashCode());
        }

        public override bool Equals(object obj)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var rhs = (MapperKey)obj;
            After:
                        var rhs = (MapperKey)obj;
            */
            MapperKey rhs = (MapperKey)obj;
            return _hash == rhs._hash && _TypeFrom == rhs._TypeFrom && _TypeTo == rhs._TypeTo && _mapperName == rhs._mapperName;
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}