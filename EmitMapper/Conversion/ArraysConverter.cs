using EmitMapper.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmitMapper.Conversion
{
    internal class ArraysConverter_DifferentTypes<TFrom, TTo> : ICustomConverter
    {
        private Func<TFrom, TTo> _converter;
        public TTo[] Convert(ICollection<TFrom> from, object state)
        {
            if (from == null)
            {
                return null;
            }

            TTo[] result = new TTo[from.Count];
            int idx = 0;
            foreach (TFrom f in from)
            {
                result[idx++] = _converter(f);
            }
            return result;
        }

        public void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig)
        {
            StaticConvertersManager staticConverters = mappingConfig.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
            System.Reflection.MethodInfo staticConverterMethod = staticConverters.GetStaticConverter(typeof(TFrom), typeof(TTo));
            if (staticConverterMethod != null)
            {
                _converter = (Func<TFrom, TTo>)Delegate.CreateDelegate(
                    typeof(Func<TFrom, TTo>),
                    null,
                    staticConverterMethod
                );
            }
            else
            {
                _subMapper = ObjectMapperManager.DefaultInstance.GetMapperInt(typeof(TFrom), typeof(TTo), mappingConfig);
                _converter = ConverterBySubmapper;
            }
        }

        private ObjectsMapperDescr _subMapper;
        private TTo ConverterBySubmapper(TFrom from)
        {
            return (TTo)_subMapper.mapper.Map(from);
        }
    }

    internal class ArraysConverter_OneTypes<T>
    {
        public T[] Convert(ICollection<T> from, object state)
        {
            return @from?.ToArray();
        }
    }

    internal class ArraysConverterProvider : ICustomConverterProvider
    {
        public CustomConverterDescriptor GetCustomConverterDescr(
            Type from,
            Type to,
            MapConfigBaseImpl mappingConfig)
        {
            Type[] tFromTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(from);
            Type[] tToTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(to);
            if (tFromTypeArgs == null || tToTypeArgs == null || tFromTypeArgs.Length != 1 || tToTypeArgs.Length != 1)
            {
                return null;
            }
            Type tFrom = tFromTypeArgs[0];
            Type tTo = tToTypeArgs[0];
            if (tFrom == tTo && (tFrom.IsValueType || mappingConfig.GetRootMappingOperation(tFrom, tTo).ShallowCopy))
            {
                return new CustomConverterDescriptor
                {
                    ConversionMethodName = "Convert",
                    ConverterImplementation = typeof(ArraysConverter_OneTypes<>),
                    ConverterClassTypeArguments = new[] { tFrom }
                };
            }

            return new CustomConverterDescriptor
            {
                ConversionMethodName = "Convert",
                ConverterImplementation = typeof(ArraysConverter_DifferentTypes<,>),
                ConverterClassTypeArguments = new[] { tFrom, tTo }
            };

        }
    }
}
