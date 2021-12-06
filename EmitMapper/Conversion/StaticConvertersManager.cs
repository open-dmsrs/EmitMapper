namespace EmitMapper.Conversion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EmitMapper.EmitInvoker.Methods;

public class StaticConvertersManager
{
    private static StaticConvertersManager _defaultInstance;

    private static readonly Dictionary<MethodInfo, Func<object, object>> _convertersFunc = new();

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
                this._typesMethods[new TypesPair { typeFrom = parameters[0].ParameterType, typeTo = m.ReturnType }] = m;
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

        this._typesMethods.TryGetValue(new TypesPair { typeFrom = from, typeTo = to }, out var res);
        return res;
    }

    public Func<object, object> GetStaticConverterFunc(Type from, Type to)
    {
        var mi = this.GetStaticConverter(from, to);
        if (mi == null)
            return null;
        lock (_convertersFunc)
        {
            if (_convertersFunc.TryGetValue(mi, out var res))
                return res;
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
            return this.typeFrom.GetHashCode() + this.typeTo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var rhs = (TypesPair)obj;
            return this.typeFrom == rhs.typeFrom && this.typeTo == rhs.typeTo;
        }

        public override string ToString()
        {
            return this.typeFrom + " -> " + this.typeTo;
        }
    }
}