using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.EmitInvoker.Methods;

namespace EmitMapper.Conversion;

public class StaticConvertersManager
{
    private static readonly Dictionary<MethodInfo, Func<object, object>> _ConvertersFunc = new();

    private static StaticConvertersManager __DefaultInstance;

    private readonly Dictionary<TypesPair, MethodInfo> _typesMethods = new();

    private readonly List<Func<Type, Type, MethodInfo>> _typesMethodsFunc = new();

    public static StaticConvertersManager DefaultInstance
    {
        get
        {
            if (__DefaultInstance == null)
                lock (typeof(StaticConvertersManager))
                {
                    if (__DefaultInstance == null)
                    {
                        __DefaultInstance = new StaticConvertersManager();
                        __DefaultInstance.AddConverterClass(typeof(Convert));
                        __DefaultInstance.AddConverterClass(typeof(EMConvert));
                        __DefaultInstance.AddConverterClass(typeof(NullableConverter));
                        __DefaultInstance.AddConverterFunc(EMConvert.GetConversionMethod);
                    }
                }

            return __DefaultInstance;
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

    private class TypesPair
    {
        public readonly Type TypeFrom;

        public readonly Type TypeTo;

        public TypesPair(Type typeFrom, Type typeTo)
        {
            TypeFrom = typeFrom;
            TypeTo = typeTo;
        }

        public override int GetHashCode()
        {
            return TypeFrom.GetHashCode() + TypeTo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var rhs = (TypesPair)obj;
            return TypeFrom == rhs.TypeFrom && TypeTo == rhs.TypeTo;
        }

        public override string ToString()
        {
            return TypeFrom + " -> " + TypeTo;
        }
    }
}