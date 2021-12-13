namespace EmitMapper.Conversion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EmitMapper.EmitInvoker.Methods;

public class StaticConvertersManager
{
    private static StaticConvertersManager __DefaultInstance;

    private static readonly Dictionary<MethodInfo, Func<object, object>> _ConvertersFunc = new();

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
                this._typesMethods[new TypesPair(parameters[0].ParameterType, m.ReturnType)] = m;
        }
    }

    public void AddConverterFunc(Func<Type, Type, MethodInfo> converterFunc)
    {
        this._typesMethodsFunc.Add(converterFunc);
    }

    public MethodInfo GetStaticConverter(Type from, Type to)
    {
        if (from == null || to == null)
            return null;

        foreach (var func in ((IEnumerable<Func<Type, Type, MethodInfo>>)this._typesMethodsFunc).Reverse())
        {
            var result = func(from, to);
            if (result != null)
                return result;
        }

        this._typesMethods.TryGetValue(new TypesPair(from, to), out var res);
        return res;
    }

    public Func<object, object> GetStaticConverterFunc(Type from, Type to)
    {
        var mi = this.GetStaticConverter(from, to);
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
            this.TypeFrom = typeFrom;
            this.TypeTo = typeTo;
        }

        public override int GetHashCode()
        {
            return this.TypeFrom.GetHashCode() + this.TypeTo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var rhs = (TypesPair)obj;
            return this.TypeFrom == rhs.TypeFrom && this.TypeTo == rhs.TypeTo;
        }

        public override string ToString()
        {
            return this.TypeFrom + " -> " + this.TypeTo;
        }
    }
}