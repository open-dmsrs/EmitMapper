using EmitMapper.EmitInvoker.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Conversion
{
    public class StaticConvertersManager
    {
        private static StaticConvertersManager _defaultInstance;

        private static readonly Dictionary<MethodInfo, Func<object, object>> _convertersFunc = new Dictionary<MethodInfo, Func<object, object>>();

        private readonly Dictionary<TypesPair, MethodInfo> _typesMethods = new Dictionary<TypesPair, MethodInfo>();
        private readonly List<Func<Type, Type, MethodInfo>> _typesMethodsFunc = new List<Func<Type, Type, MethodInfo>>();

        public static StaticConvertersManager DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (typeof(StaticConvertersManager))
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new StaticConvertersManager();
                            _defaultInstance.AddConverterClass(typeof(Convert));
                            _defaultInstance.AddConverterClass(typeof(EMConvert));
                            _defaultInstance.AddConverterClass(typeof(NullableConverter));
                            _defaultInstance.AddConverterFunc(EMConvert.GetConversionMethod);
                        }
                    }
                }
                return _defaultInstance;
            }
        }

        public void AddConverterClass(Type converterClass)
        {
            foreach (MethodInfo m in converterClass.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length == 1 && m.ReturnType != typeof(void))
                {
                    _typesMethods[
                        new TypesPair
                        {
                            typeFrom = parameters[0].ParameterType,
                            typeTo = m.ReturnType
                        }
                        ] = m;
                }
            }
        }

        public void AddConverterFunc(Func<Type, Type, MethodInfo> converterFunc)
        {
            _typesMethodsFunc.Add(converterFunc);
        }

        public MethodInfo GetStaticConverter(Type from, Type to)
        {
            if (from == null || to == null)
            {
                return null;
            }

            foreach (Func<Type, Type, MethodInfo> func in ((IEnumerable<Func<Type, Type, MethodInfo>>)_typesMethodsFunc).Reverse())
            {
                MethodInfo result = func(from, to);
                if (result != null)
                {
                    return result;
                }
            }

            _typesMethods.TryGetValue(new TypesPair { typeFrom = from, typeTo = to }, out MethodInfo res);
            return res;
        }

        public Func<object, object> GetStaticConverterFunc(Type from, Type to)
        {
            MethodInfo mi = GetStaticConverter(from, to);
            if (mi == null)
            {
                return null;
            }
            lock (_convertersFunc)
            {
                if (_convertersFunc.TryGetValue(mi, out Func<object, object> res))
                {
                    return res;
                }
                res = ((MethodInvokerFunc_1)MethodInvoker.GetMethodInvoker(null, mi)).CallFunc;
                _convertersFunc.Add(mi, res);
                return res;
            }
        }

        private class TypesPair
        {
            public Type typeFrom;
            public Type typeTo;

            public override int GetHashCode()
            {
                return typeFrom.GetHashCode() + typeTo.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                TypesPair rhs = (TypesPair)obj;
                return typeFrom == rhs.typeFrom && typeTo == rhs.typeTo;
            }

            public override string ToString()
            {
                return typeFrom + " -> " + typeTo;
            }
        }
    }
}