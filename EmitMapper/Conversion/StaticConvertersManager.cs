using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.EmitInvoker.Methods;
using EmitMapper.Utils;

namespace EmitMapper.Conversion;

public class StaticConvertersManager
{
  private static readonly Dictionary<MethodInfo, Func<object, object>> _ConvertersFunc = new();

  private static StaticConvertersManager _defaultInstance;
  private static readonly object locker = new();

  private readonly Dictionary<TypesPair, MethodInfo> _typesMethods = new();

  private readonly List<Func<Type, Type, MethodInfo>> _typesMethodsFunc = new();

  public static StaticConvertersManager DefaultInstance
  {
    get
    {
      if (_defaultInstance == null)
        lock (locker)
        {
          if (_defaultInstance == null)
          {
            _defaultInstance = new StaticConvertersManager();
            _defaultInstance.AddConverterClass(TypeHome.Convert);
            _defaultInstance.AddConverterClass(Metadata<EMConvert>.Type);
            _defaultInstance.AddConverterClass(Metadata<NullableConverter>.Type);
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
      if (parameters.Length == 1 && m.ReturnType != TypeHome.Void)
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
}