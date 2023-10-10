namespace EmitMapper.Mappers;

/// <summary>
///   Mapper for collections. It can copy Array, List&lt;&gt;, ArrayList collections.
///   Collection type in source object and destination object can differ.
/// </summary>
public class MapperForCollection : CustomMapper
{
  private static readonly MethodInfo CopyToListMethod = Metadata<MapperForCollection>.Type.GetMethod(
    nameof(CopyToList),
    BindingFlags.Instance | BindingFlags.NonPublic);

  private static readonly MethodInfo CopyToListScalarMethod = Metadata<MapperForCollection>.Type.GetMethod(
    nameof(CopyToListScalar),
    BindingFlags.Instance | BindingFlags.NonPublic);

  private static readonly LazyConcurrentDictionary<Type, bool> IsSupportedCache = new();

  private MapperDescription _subMapper;

  /// <summary>
  ///   Initializes a new instance of the <see cref="MapperForCollection" /> class.
  /// </summary>
  protected MapperForCollection()
    : base(null, null, null, null, null)
  {
  }

  /// <summary>
  ///   Creates an instance of Mapper for collections.
  /// </summary>
  /// <param name="mapperName">Mapper name. It is used for registration in Mappers repositories.</param>
  /// <param name="objectMapperManager">Mappers manager</param>
  /// <param name="typeFrom">Source type</param>
  /// <param name="typeTo">Destination type</param>
  /// <param name="subMapper"></param>
  /// <param name="mappingConfigurator"></param>
  /// <returns></returns>
  public static MapperForCollection CreateInstance(
    string mapperName,
    Mapper objectMapperManager,
    Type typeFrom,
    Type typeTo,
    MapperDescription subMapper,
    IMappingConfigurator mappingConfigurator)
  {
    var tb = DynamicAssemblyManager.DefineType("GenericListInv_" + mapperName, Metadata<MapperForCollection>.Type);

    if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinitionCache() == Metadata.List1)
    {
      var methodBuilder = tb.DefineMethod(
        nameof(CopyToListInvoke),
        MethodAttributes.Family | MethodAttributes.Virtual,
        Metadata<object>.Type,
        new[] { Metadata<IEnumerable>.Type });

      InvokeCopyImpl(typeTo, CopyToListMethod).Compile(new CompilationContext(methodBuilder.GetILGenerator()));

      methodBuilder = tb.DefineMethod(
        nameof(CopyToListScalarInvoke),
        MethodAttributes.Family | MethodAttributes.Virtual,
        Metadata<object>.Type,
        new[] { Metadata<object>.Type });

      InvokeCopyImpl(typeTo, CopyToListScalarMethod).Compile(new CompilationContext(methodBuilder.GetILGenerator()));
    }

    var result = ObjectFactory.CreateInstance<MapperForCollection>(tb.CreateType());
    result.Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, null);
    result._subMapper = subMapper;

    return result;
  }

  /// <summary>
  ///   Creates the target instance.
  /// </summary>
  /// <returns>An object.</returns>
  public override object CreateTargetInstance()
  {
    return null;
  }

  /// <summary>
  ///   Copies object properties and members of "from" to object "to"
  /// </summary>
  /// <param name="from">Source object</param>
  /// <param name="to">Destination object</param>
  /// <param name="state"></param>
  /// <returns>Destination object</returns>
  public override object Map(object from, object to, object state)
  {
    return base.Map(from, null, state);
  }

  /// <summary>
  ///   Copies object properties and members of "from" to object "to"
  /// </summary>
  /// <param name="from">Source object</param>
  /// <param name="to">Destination object</param>
  /// <param name="state"></param>
  /// <returns>Destination object</returns>
  public override object MapImpl(object from, object to, object state)
  {
    if (to == null && TargetConstructor != null)
      to = TargetConstructor.CallFunc();

    if (TypeTo.IsArray)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToArray(fromEnumerable);

      return CopyScalarToArray(from);
    }

    if (TypeTo.IsGenericType && TypeTo.GetGenericTypeDefinitionCache() == Metadata.List1)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToListInvoke(fromEnumerable);

      return CopyToListScalarInvoke(from);
    }

    if (TypeTo == Metadata<ArrayList>.Type)
    {
      if (from is IEnumerable fromEnumerable)
        return CopyToArrayList(fromEnumerable);

      return CopyToArrayListScalar(from);
    }

    if (Metadata<IList>.Type.IsAssignableFrom(TypeTo))
      return CopyToIList((IList)to, from);

    return null;
  }

  /// <summary>
  ///   Gets the sub mapper type from.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>A Type.</returns>
  internal static Type GetSubMapperTypeFrom(Type from)
  {
    var result = ExtractElementType(from);

    if (result == null)
      return from;

    return result;
  }

  /// <summary>
  ///   Gets the sub mapper type to.
  /// </summary>
  /// <param name="to">The to.</param>
  /// <returns>A Type.</returns>
  internal static Type GetSubMapperTypeTo(Type to)
  {
    return ExtractElementType(to);
  }

  /// <summary>
  ///   Returns true if specified type is supported by this Mapper
  /// </summary>
  /// <param name="t"></param>
  /// <returns></returns>
  internal static bool IsSupportedType(Type t)
  {
    return IsSupportedCache.GetOrAdd(
      t,
      type => type.IsArray || type.IsGenericType && type.GetGenericTypeDefinitionCache() == Metadata.List1
                           || type == Metadata<ArrayList>.Type || Metadata<IList>.Type.IsAssignableFrom(type)
                           || Metadata.IList1.IsAssignableFrom(type));
  }

  /// <summary>
  ///   Copies the to list.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="from">The from.</param>
  /// <returns><![CDATA[List<T>]]></returns>
  protected List<T> CopyToList<T>(IEnumerable from)
  {
    List<T> result;

    if (from is ICollection collection)
      result = new List<T>(collection.Count);
    else
      result = new List<T>();

    foreach (var obj in from)
      result.Add((T)_subMapper.Mapper.Map(obj));

    return result;
  }

  /// <summary>
  ///   Copies the to list invoke.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>An object.</returns>
  protected virtual object CopyToListInvoke(IEnumerable from)
  {
    return null;
  }

  /// <summary>
  ///   Copies the to list scalar.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="from">The from.</param>
  /// <returns><![CDATA[List<T>]]></returns>
  protected List<T> CopyToListScalar<T>(object from)
  {
    var result = new List<T>(1) { (T)_subMapper.Mapper.Map(from) };

    return result;
  }

  /// <summary>
  ///   Copies the to list scalar invoke.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>An object.</returns>
  protected virtual object CopyToListScalarInvoke(object from)
  {
    return null;
  }

  /// <summary>
  ///   Extracts the element type.
  /// </summary>
  /// <param name="collection">The collection.</param>
  /// <returns>A Type.</returns>
  private static Type ExtractElementType(Type collection)
  {
    if (collection.IsArray)
      return collection.GetElementType();

    if (collection == Metadata<ArrayList>.Type)
      return Metadata<object>.Type;

    if (collection.IsGenericType && collection.GetGenericTypeDefinitionCache() == Metadata.List1)
      return collection.GetGenericArguments()[0];

    return null;
  }

  /// <summary>
  ///   Invokes the copy impl.
  /// </summary>
  /// <param name="copiedObjectType">The copied object type.</param>
  /// <param name="copyMethod">The copy method.</param>
  /// <returns>An IAstNode.</returns>
  private static IAstNode InvokeCopyImpl(Type copiedObjectType, MethodInfo copyMethod)
  {
    var mi = copyMethod?.MakeGenericMethod(ExtractElementType(copiedObjectType));

    return new AstReturn
    {
      ReturnType = Metadata<object>.Type,
      ReturnValue = AstBuildHelper.CallMethod(
        mi,
        AstBuildHelper.ReadThis(Metadata<MapperForCollection>.Type),
        new List<IAstStackItem> { new AstReadArgumentRef { ArgumentIndex = 1, ArgumentType = Metadata<object>.Type } })
    };
  }

  /// <summary>
  ///   Copies the scalar to array.
  /// </summary>
  /// <param name="scalar">The scalar.</param>
  /// <returns>An Array.</returns>
  private Array CopyScalarToArray(object scalar)
  {
    var result = Array.CreateInstance(TypeTo.GetElementType(), 1);
    result.SetValue(_subMapper.Mapper.Map(scalar), 0);

    return result;
  }

  /// <summary>
  ///   Copies the to array.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>An Array.</returns>
  private Array CopyToArray(IEnumerable from)
  {
    if (from is ICollection collection)
    {
      var result = Array.CreateInstance(TypeTo.GetElementType(), collection.Count);
      var idx = 0;

      foreach (var obj in collection)
        result.SetValue(_subMapper.Mapper.Map(obj), idx++);

      return result;
    }
    else
    {
      var result = new ArrayList();

      foreach (var obj in from)
        result.Add(obj);

      return result.ToArray(TypeTo.GetElementType());
    }
  }

  /// <summary>
  ///   Copies the to array list.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>An ArrayList.</returns>
  private ArrayList CopyToArrayList(IEnumerable from)
  {
    if (ShallowCopy)
    {
      if (from is ICollection collection)
        return new ArrayList(collection);

      var res = new ArrayList();

      foreach (var obj in from)
        res.Add(obj);

      return res;
    }

    ArrayList result;

    if (from is ICollection coll)
      result = new ArrayList(coll.Count);
    else
      result = new ArrayList();

    foreach (var obj in from)
      if (obj == null)
      {
        result.Add(null);
      }
      else
      {
        var mapper = Mapper.GetMapper(obj.GetType(), obj.GetType(), MappingConfigurator);
        result.Add(mapper.Map(obj));
      }

    return result;
  }

  /// <summary>
  ///   Copies the to array list scalar.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <returns>An ArrayList.</returns>
  private ArrayList CopyToArrayListScalar(object from)
  {
    var result = new ArrayList(1);

    if (ShallowCopy)
    {
      result.Add(from);

      return result;
    }

    var mapper = Mapper.GetMapper(from.GetType(), from.GetType(), MappingConfigurator);
    result.Add(mapper.Map(from));

    return result;
  }

  /// <summary>
  ///   Copies the to i list.
  /// </summary>
  /// <param name="iList">The i list.</param>
  /// <param name="from">The from.</param>
  /// <returns>An object.</returns>
  private object CopyToIList(IList iList, object from)
  {
    iList ??= ObjectFactory.CreateInstance<IList>(TypeTo);

    foreach (var obj in from is IEnumerable fromEnumerable ? fromEnumerable : from.AsEnumerable())
      if (obj == null)
      {
        iList.Add(null);
      }
      else if (RootOperation == null || RootOperation.ShallowCopy)
      {
        iList.Add(obj);
      }
      else
      {
        var mapper = Mapper.GetMapper(obj.GetType(), obj.GetType(), MappingConfigurator);
        iList.Add(mapper.Map(obj));
      }

    return iList;
  }
}