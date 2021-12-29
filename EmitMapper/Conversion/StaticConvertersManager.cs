using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.EmitInvoker.Methods;

namespace EmitMapper.Conversion;

public class StaticConvertersManager
{
    private static readonly Dictionary<MethodInfo, Func<object, object>> _ConvertersFunc = new();

    private static StaticConvertersManager _defaultInstance;

    private readonly Dictionary<TypesPair, MethodInfo> _typesMethods = new();

    private readonly List<Func<Type, Type, MethodInfo>> _typesMethodsFunc = new();

    public static StaticConvertersManager DefaultInstance
    {
        get
        {
            if (_defaultInstance == null)
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

            return _defaultInstance;
        }
    }

    public void AddConverterClass(Type converterClass)
    {
        foreach (var m in converterClass.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            var parameters = m.GetParameters();
            if (parameters.Length == 1 && m.ReturnType != typeof(void))
                _typesMethods[new TypesPair(parameters[0].ParameterType, m.ReturnType)] = m;
        }
    }

    public void AddConverterFunc(Func<Type, Type, MethodInfo> converterFunc)
    {
        _typesMethodsFunc.Add(converterFunc);
    }

    public MethodInfo GetStaticConverter(Type from, Type to)
    {
        if (from == null || to == null)
            return null;

        foreach (var func in ((IEnumerable<Func<Type, Type, MethodInfo>>)_typesMethodsFunc).Reverse())
        {
            var result = func(from, to);
            if (result != null)
                return result;
        }

        _typesMethods.TryGetValue(new TypesPair(from, to), out var res);
        return res;
    }

    public Func<object, object> GetStaticConverterFunc(Type from, Type to)
    {
        var mi = GetStaticConverter(from, to);
        if (mi == null)
            return null;
        lock (_ConvertersFunc)
        {
            if (_ConvertersFunc.TryGetValue(mi, out var res))
                return res;
            res = ((MethodInvokerFunc1)MethodInvoker.GetMethodInvoker(null, mi)).CallFunc;
            _ConvertersFunc.Add(mi, res);
            return res;
        }
    }

    private class TypesPair : IEqualityComparer<TypesPair>, IEquatable<TypesPair>
    {
        public readonly Type TypeFrom;

        public readonly Type TypeTo;

        private readonly int _hash;

        public TypesPair(Type typeFrom, Type typeTo)
        {
            TypeFrom = typeFrom;
            TypeTo = typeTo;

            _hash = HashCode.Combine(typeFrom, typeTo);
        }

        public bool Equals(TypesPair x, TypesPair y)
        {
            if (x != null)
                return x.Equals(y);
            if (y != null)
                return y.Equals(x);
            return true;
        }

        public int GetHashCode(TypesPair obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(TypesPair other)
        {
            if (other == null) return false;
            return _hash == other._hash && TypeFrom == other.TypeFrom && TypeTo == other.TypeTo;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TypesPair);
        }

        public override string ToString()
        {
            return TypeFrom + " -> " + TypeTo;
        }
    }
}