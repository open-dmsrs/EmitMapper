using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.Conversion;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.MappingConfiguration;

public class DefaultMapConfig : MapConfigBaseImpl
{
  private readonly List<string> _deepCopyMembers = new();

  private readonly List<string> _shallowCopyMembers = new();

  private string _configName;

  private Func<string, string, bool> _membersMatcher;

  private bool _shallowCopy;

  static DefaultMapConfig()
  {
    Instance = new DefaultMapConfig();
  }

  public DefaultMapConfig()
  {
    _shallowCopy = true;
    _membersMatcher = (m1, m2) => m1 == m2;
  }

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

  public override string GetConfigurationName()
  {
    return _configName ??= base.GetConfigurationName() + new[]
    {
      _shallowCopy.ToString(), ToStr(_membersMatcher), ToStrEnum(_shallowCopyMembers), ToStrEnum(_deepCopyMembers)
    }.ToCsv(";");
  }

  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    return FilterOperations(from, to, GetMappingItems(new HashSet<TypesPair>(), from, to, null, null));
  }

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

  protected virtual bool MatchMembers(string m1, string m2)
  {
    return _membersMatcher(m1, m2);
  }

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

  private bool IsShallowCopy(Type from, Type to)
  {
    if (TypeInList(_shallowCopyMembers, to) || TypeInList(_shallowCopyMembers, from))
      return true;

    if (TypeInList(_deepCopyMembers, to) || TypeInList(_deepCopyMembers, from))
      return false;

    return _shallowCopy;
  }

  private bool IsShallowCopy(MemberDescriptor from, MemberDescriptor to)
  {
    return IsShallowCopy(from.MemberType, to.MemberType);
  }

  private bool MappingItemNameInList(IEnumerable<string> list, ReadWriteSimple mo)
  {
    var enumerable = list.ToList();

    return enumerable.Any(l => MatchMembers(l, mo.Destination.MemberInfo.Name))
           || enumerable.Any(l => MatchMembers(l, mo.Source.MemberInfo.Name));
  }

  private bool MappingItemTypeInList(IEnumerable<string> list, ReadWriteSimple mo)
  {
    var enumerable = list.ToList();

    return TypeInList(enumerable, mo.Destination.MemberType) || TypeInList(enumerable, mo.Source.MemberType);
  }

  private bool TypeInList(IEnumerable<string> list, Type t)
  {
    return list.Any(l => MatchMembers(l, t.FullName));
  }
}