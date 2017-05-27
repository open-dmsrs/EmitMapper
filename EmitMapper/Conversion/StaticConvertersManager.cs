using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.Conversion;
using EmitMapper.EmitInvoker;

namespace EmitMapper
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
                    lock (typeof (StaticConvertersManager))
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new StaticConvertersManager();
                            _defaultInstance.AddConverterClass(typeof (Convert));
                            _defaultInstance.AddConverterClass(typeof (EMConvert));
                            _defaultInstance.AddConverterClass(typeof (NullableConverter));
                            _defaultInstance.AddConverterFunc(EMConvert.GetConversionMethod);
                        }
                    }
                }
                return _defaultInstance;
            }
        }

        public void AddConverterClass(Type converterClass)
        {
            foreach (var m in converterClass.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                var parameters = m.GetParameters();
                if (parameters.Length == 1 && m.ReturnType != typeof (void))
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

            foreach (var func in ((IEnumerable<Func<Type, Type, MethodInfo>>) _typesMethodsFunc).Reverse())
            {
                var result = func(from, to);
                if (result != null)
                {
                    return result;
                }
            }

            MethodInfo res = null;
            _typesMethods.TryGetValue(new TypesPair {typeFrom = from, typeTo = to}, out res);
            return res;
        }

        public Func<object, object> GetStaticConverterFunc(Type from, Type to)
        {
            var mi = GetStaticConverter(from, to);
            if (mi == null)
            {
                return null;
            }
            lock (_convertersFunc)
            {
                Func<object, object> res = null;
                if (_convertersFunc.TryGetValue(mi, out res))
                {
                    return res;
                }
                res = ((MethodInvokerFunc_1) MethodInvoker.GetMethodInvoker(null, mi)).CallFunc;
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
                var rhs = (TypesPair) obj;
                return typeFrom == rhs.typeFrom && typeTo == rhs.typeTo;
            }

            public override string ToString()
            {
                return typeFrom + " -> " + typeTo;
            }
        }
    }
}