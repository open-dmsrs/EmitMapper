namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The default map config.
/// </summary>
public class DefaultMapConfig : MapConfigBaseImpl
{
  private readonly List<string> _deepCopyMembers = new();

  private readonly List<string> _shallowCopyMembers = new();

  private string _configName;

  private Func<string, string, bool> _membersMatcher;

  private bool _shallowCopy;

  /// <summary>
  ///   Initializes a new instance of the <see cref="DefaultMapConfig" /> class.
  /// </summary>
  static DefaultMapConfig()
  {
    Instance = new DefaultMapConfig();
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="DefaultMapConfig" /> class.
  /// </summary>
  public DefaultMapConfig()
  {
    _shallowCopy = true;
    _membersMatcher = (m1, m2) => m1 == m2;
  }

  /// <summary>
  ///   Gets the instance.
  /// </summary>
  public static DefaultMapConfig Instance { get; }

  /// <summary>
  ///   Define deep map mode for the specified type. In that case all members of this type will be copied by value (new
  ///   instances will be created)
  /// </summary>
  /// <typeparam name="T">Type for which deep map mode is defining</typeparam>
  /// <returns></returns>
  public DefaultMapConfig DeepMap<T>()
  {
    return DeepMap(Metadata<T>.Type);
  }

  /// <summary>
  ///   Define deep map mode for the specified type. In that case all members of this type will be copied by value (new
  ///   instances will be created)
  /// </summary>
  /// <param name="type">Type for which deep map mode is defining</param>
  /// <returns></returns>
  public DefaultMapConfig DeepMap(Type type)
  {
    _deepCopyMembers.Add(type.FullName);

    return this;
  }

  /// <summary>
  ///   Define default deep map mode. In that case all members will be copied by value (new instances will be created) by
  ///   default
  /// </summary>
  /// <returns></returns>
  public DefaultMapConfig DeepMap()
  {
    _shallowCopy = false;

    return this;
  }

  /// <summary>
  ///   Gets the configuration name.
  /// </summary>
  /// <returns>A string.</returns>
  public override string GetConfigurationName()
  {
    return _configName ??= base.GetConfigurationName() + new[]
    {
      _shallowCopy.ToString(), ToStr(_membersMatcher), ToStrEnum(_shallowCopyMembers), ToStrEnum(_deepCopyMembers)
    }.ToCsv(";");
  }

  /// <summary>
  ///   Gets the mapping operations.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns><![CDATA[IEnumerable<IMappingOperation>]]></returns>
  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    return FilterOperations(from, to, GetMappingItems(new HashSet<TypesPair>(), from, to, null, null));
  }

  /// <summary>
  ///   Gets the root mapping operation.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns>An IRootMappingOperation.</returns>
  public override IRootMappingOperation GetRootMappingOperation(Type from, Type to)
  {
    var res = base.GetRootMappingOperation(from, to);
    res.ShallowCopy = IsShallowCopy(from, to);

    return res;
  }

  /// <summary>
  ///   Define a function to test two members if they have identical names.
  /// </summary>
  /// <param name="membersMatcher">
  ///   Function to test two members if they have identical names. For example if you want to
  ///   match members ignoring case you can define the following function: (m1, m2) => m1.ToUpper() == m2.ToUpper()
  /// </param>
  /// <returns></returns>
  public DefaultMapConfig MatchMembers(Func<string, string, bool> membersMatcher)
  {
    _membersMatcher = membersMatcher;

    return this;
  }

  /// <summary>
  ///   Define shallow map mode for the specified type. In that case all members of this type will be copied by reference
  ///   if it is possible
  /// </summary>
  /// <typeparam name="T">Type for which shallow map mode is defining</typeparam>
  /// <returns></returns>
  public DefaultMapConfig ShallowMap<T>()
  {
    return ShallowMap(Metadata<T>.Type);
  }

  /// <summary>
  ///   Define shallow map mode for the specified type. In that case all members of this type will be copied by reference
  ///   if it is possible
  /// </summary>
  /// <param name="type">Type for which shallow map mode is defining</param>
  /// <returns></returns>
  public DefaultMapConfig ShallowMap(Type type)
  {
    _shallowCopyMembers.Add(type.FullName);

    return this;
  }

  /// <summary>
  ///   Define default shallow map mode. In that case all members will be copied by reference (if it is possible) by
  ///   default.
  /// </summary>
  /// <returns></returns>
  public DefaultMapConfig ShallowMap()
  {
    _shallowCopy = true;

    return this;
  }

  /// <summary>
  ///   Match members.
  /// </summary>
  /// <param name="m1">The m1.</param>
  /// <param name="m2">The m2.</param>
  /// <returns>A bool.</returns>
  protected virtual bool MatchMembers(string m1, string m2)
  {
    return _membersMatcher(m1, m2);
  }

  /// <summary>
  ///   Are the native deep copy.
  /// </summary>
  /// <param name="typeFrom">The type from.</param>
  /// <param name="typeTo">The type to.</param>
  /// <param name="fromMi">The from mi.</param>
  /// <param name="toMi">The to mi.</param>
  /// <param name="shallowCopy">If true, shallow copy.</param>
  /// <returns>A bool.</returns>
  private static bool IsNativeDeepCopy(Type typeFrom, Type typeTo, MemberInfo fromMi, MemberInfo toMi, bool shallowCopy)
  {
    if (NativeConverter.IsNativeConvertionPossible(typeFrom, typeTo))
      return false;

    if (MapperForCollection.IsSupportedType(typeFrom) || MapperForCollection.IsSupportedType(typeTo))
      return false;

    if (typeTo != typeFrom || !shallowCopy)
      return true;

    return false;
  }

  /// <summary>
  ///   Creates the mapping operation.
  /// </summary>
  /// <param name="processedTypes">The processed types.</param>
  /// <param name="fromRoot">The from root.</param>
  /// <param name="toRoot">The to root.</param>
  /// <param name="toPath">The to path.</param>
  /// <param name="fromPath">The from path.</param>
  /// <param name="fromMi">The from mi.</param>
  /// <param name="toMi">The to mi.</param>
  /// <returns>An IMappingOperation.</returns>
  private IMappingOperation CreateMappingOperation(
    HashSet<TypesPair> processedTypes,
    Type fromRoot,
    Type toRoot,
    IEnumerable<MemberInfo> toPath,
    IEnumerable<MemberInfo> fromPath,
    MemberInfo fromMi,
    MemberInfo toMi)
  {
    var memberInfos = toPath.ToList();
    var origDestMemberDesc = new MemberDescriptor(memberInfos.Concat(new[] { toMi }));
    var enumerable = fromPath.ToList();
    var origSrcMemberDesc = new MemberDescriptor(enumerable.Concat(new[] { fromMi }));

    if (ReflectionHelper.IsNullable(ReflectionHelper.GetMemberReturnType(fromMi)))

      // fromPath = enumerable.Concat(new[] { fromMi });//never use
      fromMi = ReflectionHelper.GetMemberReturnType(fromMi).GetProperty("Value");

    if (ReflectionHelper.IsNullable(ReflectionHelper.GetMemberReturnType(toMi)))

      // toPath = enumerable.Concat(new[] { toMi });//never use
      toMi = ReflectionHelper.GetMemberReturnType(toMi).GetProperty("Value");

    var destMemberDescr = new MemberDescriptor(memberInfos.Concat(new[] { toMi }));
    var srcMemberDescr = new MemberDescriptor(enumerable.Concat(new[] { fromMi }));
    var typeFromMember = srcMemberDescr.MemberType;
    var typeToMember = destMemberDescr.MemberType;

    var shallowCopy = IsShallowCopy(srcMemberDescr, destMemberDescr);

    if (IsNativeDeepCopy(
          typeFromMember,
          typeToMember,
          srcMemberDescr.MemberInfo,
          destMemberDescr.MemberInfo,
          shallowCopy) && !processedTypes.Contains(new TypesPair(typeFromMember, typeToMember)))
      return new ReadWriteComplex
      {
        Destination = origDestMemberDesc,
        Source = origSrcMemberDesc,
        ShallowCopy = shallowCopy,
        Operations = GetMappingItems(
          processedTypes,
          srcMemberDescr.MemberType,
          destMemberDescr.MemberType,
          null,
          null)
      };

    return new ReadWriteSimple
    {
      Source = origSrcMemberDesc, Destination = origDestMemberDesc, ShallowCopy = shallowCopy
    };
  }

  /// <summary>
  ///   Gets the mapping items.
  /// </summary>
  /// <param name="processedTypes">The processed types.</param>
  /// <param name="fromRoot">The from root.</param>
  /// <param name="toRoot">The to root.</param>
  /// <param name="toPath">The to path.</param>
  /// <param name="fromPath">The from path.</param>
  /// <returns><![CDATA[List<IMappingOperation>]]></returns>
  private List<IMappingOperation> GetMappingItems(
    HashSet<TypesPair> processedTypes,
    Type fromRoot,
    Type toRoot,
    IEnumerable<MemberInfo> toPath,
    IEnumerable<MemberInfo> fromPath)
  {
    toPath ??= Array.Empty<MemberInfo>();
    fromPath ??= Array.Empty<MemberInfo>();

    var membersFromPath = fromPath.ToArray();

    var from = membersFromPath.Length == 0
      ? fromRoot
      : ReflectionHelper.GetMemberReturnType(membersFromPath[membersFromPath.Length - 1]);

    var memberToPath = toPath.ToArray();

    var to = memberToPath.Length == 0
      ? toRoot
      : ReflectionHelper.GetMemberReturnType(memberToPath[memberToPath.Length - 1]);

    var tp = new TypesPair(from, to);
    processedTypes.Add(tp);

    var toMembers = ReflectionHelper.GetPublicFieldsAndProperties(to);
    var fromMembers = ReflectionHelper.GetPublicFieldsAndProperties(from);

    var result = new List<IMappingOperation>();

    foreach (var toMi in toMembers)
    {
      if (toMi.MemberType == MemberTypes.Property)
      {
        var setMethod = ((PropertyInfo)toMi).GetSetMethod();

        if (setMethod == null || setMethod.GetParameters().Length != 1)
          continue;
      }

      var fromMi = fromMembers.FirstOrDefault(mi => MatchMembers(mi.Name, toMi.Name));

      if (fromMi == null)
        continue;

      if (fromMi.MemberType == MemberTypes.Property)
      {
        var getMethod = ((PropertyInfo)fromMi).GetGetMethod();

        if (getMethod == null)
          continue;
      }

      var op = CreateMappingOperation(processedTypes, fromRoot, toRoot, memberToPath, membersFromPath, fromMi, toMi);

      if (op != null)
        result.Add(op);
    }

    processedTypes.Remove(tp);

    return result;
  }

  /// <summary>
  ///   Are the shallow copy.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns>A bool.</returns>
  private bool IsShallowCopy(Type from, Type to)
  {
    if (TypeInList(_shallowCopyMembers, to) || TypeInList(_shallowCopyMembers, from))
      return true;

    if (TypeInList(_deepCopyMembers, to) || TypeInList(_deepCopyMembers, from))
      return false;

    return _shallowCopy;
  }

  /// <summary>
  ///   Are the shallow copy.
  /// </summary>
  /// <param name="from">The from.</param>
  /// <param name="to">The to.</param>
  /// <returns>A bool.</returns>
  private bool IsShallowCopy(MemberDescriptor from, MemberDescriptor to)
  {
    return IsShallowCopy(from.MemberType, to.MemberType);
  }

  /// <summary>
  ///   Mappings the item name in list.
  /// </summary>
  /// <param name="list">The list.</param>
  /// <param name="mo">The mo.</param>
  /// <returns>A bool.</returns>
  private bool MappingItemNameInList(IEnumerable<string> list, ReadWriteSimple mo)
  {
    var enumerable = list.ToList();

    return enumerable.Any(l => MatchMembers(l, mo.Destination.MemberInfo.Name))
           || enumerable.Any(l => MatchMembers(l, mo.Source.MemberInfo.Name));
  }

  /// <summary>
  ///   Mappings the item type in list.
  /// </summary>
  /// <param name="list">The list.</param>
  /// <param name="mo">The mo.</param>
  /// <returns>A bool.</returns>
  private bool MappingItemTypeInList(IEnumerable<string> list, ReadWriteSimple mo)
  {
    var enumerable = list.ToList();

    return TypeInList(enumerable, mo.Destination.MemberType) || TypeInList(enumerable, mo.Source.MemberType);
  }

  /// <summary>
  ///   Types the in list.
  /// </summary>
  /// <param name="list">The list.</param>
  /// <param name="t">The t.</param>
  /// <returns>A bool.</returns>
  private bool TypeInList(IEnumerable<string> list, Type t)
  {
    return list.Any(l => MatchMembers(l, t.FullName));
  }
}