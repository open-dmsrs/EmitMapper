// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 08-21-2013
//
// Last Modified By : tangjingbo
// Last Modified On : 08-21-2013
// ***********************************************************************
// <copyright file="DataReaderToObjectMapper.cs" company="Extendsoft">
//     Copyright (c) Extendsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EmitMapper;
using EmitMapper.Mappers;
using System.Data.Common;
using System.Reflection;
using LightDataAccess.MappingConfigs;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration;
using System.Text;

namespace LightDataAccess
{
    /// <summary>
    /// Class DataReaderToObjectMapper
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataReaderToObjectMapper<TEntity> : ObjectsMapper<IDataReader, TEntity>
    {
        /// <summary>
        /// Class DbReaderMappingConfig
        /// </summary>
        class DbReaderMappingConfig : IMappingConfigurator
        {
            /// <summary>
            /// Class ReaderValuesExtrator
            /// </summary>
            /// <typeparam name="T"></typeparam>
            class ReaderValuesExtrator<T>
            {
                /// <summary>
                /// The value extractor
                /// </summary>
                public Func<int, IDataReader, T> valueExtractor;
                /// <summary>
                /// The field num
                /// </summary>
                public int fieldNum;
                /// <summary>
                /// The field name
                /// </summary>
                public string fieldName;


                /// <summary>
                /// Initializes a new instance of the ReaderValuesExtrator`1 class.
                /// </summary>
                /// <param name="fieldName">Name of the field.</param>
                /// <param name="valueExtractor">The value extractor.</param>
                public ReaderValuesExtrator(string fieldName, Func<int, IDataReader, T> valueExtractor)
                {
                    fieldNum = -1;
                    this.fieldName = fieldName;
                    this.valueExtractor = valueExtractor;
                }

                /// <summary>
                /// Gets the extration delegate.
                /// </summary>
                /// <value>The extration delegate.</value>
                public Delegate ExtrationDelegate
                {
                    get
                    {
                        return (ValueGetter<T>)
                            (
                                (value, state) =>
                                {
                                    return ValueToWrite<T>.ReturnValue(GetValue((IDataReader)state));
                                }
                            );
                    }
                }

                /// <summary>
                /// Gets the value.
                /// </summary>
                /// <param name="reader">The reader.</param>
                /// <returns>`0.</returns>
                private T GetValue(IDataReader reader)
                {
                    if (fieldNum == -1)
                    {
                        fieldNum = reader.GetOrdinal(fieldName);
                    }
                    return reader.IsDBNull(fieldNum) ? default(T) : valueExtractor(fieldNum, reader);
                }
            }

            /// <summary>
            /// The _skip fields
            /// </summary>
            IEnumerable<string> _skipFields;
            /// <summary>
            /// The _mapping key
            /// </summary>
            string _mappingKey;

            /// <summary>
            /// Initializes a new instance of the <see cref="DbReaderMappingConfig"/> class.
            /// </summary>
            /// <param name="skipFields">The skip fields.</param>
            /// <param name="mappingKey">The mapping key.</param>
            public DbReaderMappingConfig(IEnumerable<string> skipFields, string mappingKey)
            {
                _skipFields = skipFields == null ? new List<string>() : skipFields;
                _mappingKey = mappingKey;
            }

            /// <summary>
            /// Gets the root mapping operation.
            /// </summary>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            /// <returns>IRootMappingOperation.</returns>
            public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
            {
                return null;
            }

            /// <summary>
            /// Gets the values getter.
            /// </summary>
            /// <param name="ind">The ind.</param>
            /// <param name="m">The m.</param>
            /// <returns>Delegate.</returns>
            /// <exception cref="EmitMapper.EmitMapperException">Could not convert an object to  + memberType.ToString()</exception>
            private Delegate GetValuesGetter(int ind, MemberInfo m)
            {
                var memberType = ReflectionUtils.GetMemberType(m);

                if (_mappingKey != null)
                {
                    if (memberType == typeof(string))
                    {
                        return new ReaderValuesExtrator<string>(m.Name, (idx, reader) => reader.IsDBNull(idx) ? null : reader.GetString(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(bool))
                    {
                        return new ReaderValuesExtrator<bool>(m.Name, (idx, reader) => reader.GetBoolean(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(bool?))
                    {
                        return new ReaderValuesExtrator<bool?>(m.Name, (idx, reader) => reader.GetBoolean(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int16))
                    {
                        return new ReaderValuesExtrator<Int16>(m.Name, (idx, reader) => reader.GetInt16(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int16?))
                    {
                        return new ReaderValuesExtrator<Int16?>(m.Name, (idx, reader) => reader.GetInt16(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int32))
                    {
                        return new ReaderValuesExtrator<Int32>(m.Name, (idx, reader) => reader.GetInt32(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int32?))
                    {
                        return new ReaderValuesExtrator<Int32?>(m.Name, (idx, reader) => reader.GetInt32(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int64))
                    {
                        return new ReaderValuesExtrator<Int64>(m.Name, (idx, reader) => reader.GetInt64(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Int64?))
                    {
                        return new ReaderValuesExtrator<Int64?>(m.Name, (idx, reader) => reader.GetInt64(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(byte))
                    {
                        return new ReaderValuesExtrator<byte>(m.Name, (idx, reader) => reader.GetByte(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(byte?))
                    {
                        return new ReaderValuesExtrator<byte?>(m.Name, (idx, reader) => reader.GetByte(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(char))
                    {
                        return new ReaderValuesExtrator<char>(m.Name, (idx, reader) => reader.GetChar(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(char?))
                    {
                        return new ReaderValuesExtrator<char?>(m.Name, (idx, reader) => reader.GetChar(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(DateTime))
                    {
                        return new ReaderValuesExtrator<DateTime>(m.Name, (idx, reader) => reader.GetDateTime(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(DateTime?))
                    {
                        return new ReaderValuesExtrator<DateTime?>(m.Name, (idx, reader) => reader.GetDateTime(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(decimal))
                    {
                        return new ReaderValuesExtrator<decimal>(m.Name, (idx, reader) => reader.GetDecimal(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(decimal?))
                    {
                        return new ReaderValuesExtrator<decimal?>(m.Name, (idx, reader) => reader.GetDecimal(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(double))
                    {
                        return new ReaderValuesExtrator<double>(m.Name, (idx, reader) => reader.GetDouble(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(double?))
                    {
                        return new ReaderValuesExtrator<double?>(m.Name, (idx, reader) => reader.GetDouble(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(float))
                    {
                        return new ReaderValuesExtrator<float>(m.Name, (idx, reader) => reader.GetFloat(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(float?))
                    {
                        return new ReaderValuesExtrator<float?>(m.Name, (idx, reader) => reader.GetFloat(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Guid))
                    {
                        return new ReaderValuesExtrator<Guid>(m.Name, (idx, reader) => reader.GetGuid(idx)).ExtrationDelegate;
                    }
                    else if (memberType == typeof(Guid?))
                    {
                        return new ReaderValuesExtrator<Guid?>(m.Name, (idx, reader) => reader.GetGuid(idx)).ExtrationDelegate;
                    }
                }

                Func<object, object> converter = StaticConvertersManager.DefaultInstance.GetStaticConverterFunc(typeof(object), memberType);
                if (converter == null)
                {
                    throw new EmitMapperException("Could not convert an object to " + memberType.ToString());
                }
                int fieldNum = -1;
                string fieldName = m.Name;
                return
                    (ValueGetter<object>)
                    (
                        (value, state) =>
                        {
                            var reader = ((IDataReader)state);
                            object result = null;
                            if (_mappingKey != null)
                            {
                                if (fieldNum == -1)
                                {
                                    fieldNum = reader.GetOrdinal(fieldName);
                                }
                                result = reader[fieldNum];
                            }
                            else
                            {
                                result = reader[fieldName];
                            }

                            if (result is DBNull)
                            {
                                return ValueToWrite<object>.ReturnValue(null);
                            }
                            return ValueToWrite<object>.ReturnValue(converter(result));
                        }
                    )
                    ;
            }

            /// <summary>
            /// Get list of mapping operations. Each mapping mapping defines one copieng operation from source to destination. For this operation can be additionally defined the following custom operations:
            /// - Custom getter which extracts values from source
            /// - Custom values converter which converts extracted from source value
            /// - Custom setter which writes value to destination
            /// </summary>
            /// <param name="from">Source type</param>
            /// <param name="to">Destination type</param>
            /// <returns>IMappingOperation[][].</returns>
            public IMappingOperation[] GetMappingOperations(Type from, Type to)
            {
                return ReflectionUtils
                    .GetPublicFieldsAndProperties(to)
                    .Where(
                        m => m.MemberType == MemberTypes.Field ||
                            m.MemberType == MemberTypes.Property && ((PropertyInfo)m).GetSetMethod() != null
                    )
                    .Where(m => !_skipFields.Select(sf => sf.ToUpper()).Contains(m.Name.ToUpper()))
                    .Select(
                        (m, ind) =>
                            new DestWriteOperation()
                            {
                                Destination = new MemberDescriptor(new[] { m }),
                                Getter = GetValuesGetter(ind, m)
                            }
                    )
                    .ToArray<IMappingOperation>();
            }

            /// <summary>
            /// Get unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
            /// </summary>
            /// <returns>System.String.</returns>
            public string GetConfigurationName()
            {
                if (_mappingKey != null)
                {
                    return "dbreader_" + _mappingKey;
                }
                else
                {
                    return "dbreader_";
                }
            }

            /// <summary>
            /// Gets the static converters manager.
            /// </summary>
            /// <returns>StaticConvertersManager.</returns>
            public StaticConvertersManager GetStaticConvertersManager()
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderToObjectMapper{TEntity}"/> class.
        /// </summary>
        /// <param name="mappingKey">The mapping key.</param>
        /// <param name="mapperManager">The mapper manager.</param>
        /// <param name="skipFields">The skip fields.</param>
        public DataReaderToObjectMapper(
            string mappingKey,
            ObjectMapperManager mapperManager,
            IEnumerable<string> skipFields)
            : base(GetMapperImpl(mappingKey, mapperManager, skipFields))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderToObjectMapper{TEntity}"/> class.
        /// </summary>
        /// <param name="mapperManager">The mapper manager.</param>
        public DataReaderToObjectMapper(ObjectMapperManager mapperManager)
            : this(null, mapperManager, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderToObjectMapper{TEntity}"/> class.
        /// </summary>
        public DataReaderToObjectMapper()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderToObjectMapper{TEntity}"/> class.
        /// </summary>
        /// <param name="skipFields">The skip fields.</param>
        public DataReaderToObjectMapper(IEnumerable<string> skipFields)
            : this(null, null, skipFields)
        {
        }

        /// <summary>
        /// Reads the single.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>`0.</returns>
        public TEntity ReadSingle(IDataReader reader)
        {
            return ReadSingle(reader, null);
        }

        /// <summary>
        /// Reads the single.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="changeTracker">The change tracker.</param>
        /// <returns>`0.</returns>
        public TEntity ReadSingle(IDataReader reader, ObjectsChangeTracker changeTracker)
        {
            TEntity result = reader.Read() ? MapUsingState(reader, reader) : default(TEntity);
            if (changeTracker != null)
            {
                changeTracker.RegisterObject(result);
            }
            return result;
        }

        /// <summary>
        /// Reads the collection.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>IEnumerable{`0}.</returns>
        public IEnumerable<TEntity> ReadCollection(IDataReader reader)
        {
            return ReadCollection(reader, null);
        }

        /// <summary>
        /// Reads the collection.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="changeTracker">The change tracker.</param>
        /// <returns>IEnumerable{`0}.</returns>
        public IEnumerable<TEntity> ReadCollection(IDataReader reader, ObjectsChangeTracker changeTracker)
        {
            while (reader.Read())
            {
                TEntity result = MapUsingState(reader, reader);
                if (changeTracker != null)
                {
                    changeTracker.RegisterObject(result);
                }
                yield return result;
            }
        }

        /// <summary>
        /// Gets the mapper impl.
        /// </summary>
        /// <param name="mappingKey">The mapping key.</param>
        /// <param name="mapperManager">The mapper manager.</param>
        /// <param name="skipFields">The skip fields.</param>
        /// <returns>ObjectsMapperBaseImpl.</returns>
        private static ObjectsMapperBaseImpl GetMapperImpl(
            string mappingKey,
            ObjectMapperManager mapperManager,
            IEnumerable<string> skipFields)
        {
            IMappingConfigurator config = new DbReaderMappingConfig(skipFields, mappingKey);

            if (mapperManager != null)
            {
                return mapperManager.GetMapperImpl(
                    typeof(IDataReader),
                    typeof(TEntity),
                    config);
            }
            else
            {
                return ObjectMapperManager.DefaultInstance.GetMapperImpl(
                    typeof(IDataReader),
                    typeof(TEntity),
                    config);
            }
        }
    }
}
