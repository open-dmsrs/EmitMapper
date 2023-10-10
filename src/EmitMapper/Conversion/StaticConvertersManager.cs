namespace EmitMapper.Conversion;

/// <summary>
///   The static converters manager.
/// </summary>
public class StaticConvertersManager
{
  private static readonly LazyConcurrentDictionary<MethodInfo, Func<object, object>> _ConvertersFunc = new();

  private static readonly object locker = new();

  private static StaticConvertersManager _defaultInstance;

  private readonly LazyConcurrentDictionary<TypesPair, MethodInfo> _typesMethods = new();

  private readonly List<Func<Type, Type, MethodInfo>> _typesMethodsFunc = new();

  /// <summary>
  ///   Gets the default instance.
  /// </summary>
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
            _defaultInstance.AddConverterClass(Metadata.Convert);
            _defaultInstance.AddConverterClass(Metadata<EMConvert>.Type);
            _defaultInstance.AddConverterClass(Metadata<NullableConverter>.Type);
            _defaultInstance.AddConverterFunc(EMConvert.GetConversionMethod);
          }
        }

      return _defaultInstance;
    }
  }

  /// <summary>
  ///   Adds the converter class.
  /// </summary>
  /// <param name="converterClass">The converter class.</param>
  public void AddConverterClass(Type converterClass)
  {
    foreach (var m in converterClass.GetMethods(BindingFlags.Static | BindingFlags.Public))
    {
      var parameters = m.GetParameters();

      if (parameters.Length == 1 && m.ReturnType != Metadata.Void)
        _typesMethods.TryAdd(new TypesPair(parameters[0].ParameterType, m.ReturnType), m);
    }
  }

  /// <summary>
  ///   Adds the converter func.
  /// </summary>
  /// <param name="converterFunc">The converter func.</param>
  public void AddConverterFunc(Func<Type, Type, MethodInfo> converterFunc)
  {
    _typesMethodsFunc.Add(converterFunc);
  }

  /// <summary>
  ///   Gets the static converter.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns>A MethodInfo.</returns>
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

  /// <summary>
  ///   Gets the static converter func.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns><![CDATA[Func<object, object>]]></returns>
  public Func<object, object> GetStaticConverterFunc(Type from, Type to)
  {
    var mi = GetStaticConverter(from, to);

    if (mi == null)
      return null;

    return _ConvertersFunc.GetOrAdd(mi, m => ((MethodInvokerFunc1)MethodInvoker.GetMethodInvoker(null, m)).CallFunc);
  }
}