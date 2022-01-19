using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

/// <summary>
///   Contains cached reflection information for easy retrieval
/// </summary>
[DebuggerDisplay("{Type}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class TypeDetails
{
  private ConstructorParameters[] _constructors;
  private Dictionary<string, MemberInfo> _nameToMember;
  private MemberInfo[] _readAccessors;
  private MemberInfo[] _writeAccessors;

  public TypeDetails(Type type, ProfileMap config)
  {
    Type = type;
    Config = config;
  }

  public Type Type { get; }
  public ProfileMap Config { get; }
  public MemberInfo[] ReadAccessors => _readAccessors ??= BuildReadAccessors();
  public MemberInfo[] WriteAccessors => _writeAccessors ??= BuildWriteAccessors();
  public ConstructorParameters[] Constructors => _constructors ??= GetConstructors();


  private ConstructorParameters[] GetConstructors()
  {
    return GetConstructors(Type, Config).Where(c => c.ParametersCount > 0).OrderByDescending(c => c.ParametersCount)
      .ToArray();
  }

  public static IEnumerable<ConstructorParameters> GetConstructors(Type type, ProfileMap profileMap)
  {
    return type.GetDeclaredConstructors().Where(profileMap.ShouldUseConstructor)
      .Select(c => new ConstructorParameters(c));
  }

  public MemberInfo GetMember(string name)
  {
    _nameToMember ??= PossibleNames();
    return _nameToMember.GetOrDefault(name);
  }

  private Dictionary<string, MemberInfo> PossibleNames()
  {
    var nameToMember = new Dictionary<string, MemberInfo>(ReadAccessors.Length, StringComparer.OrdinalIgnoreCase);
    IEnumerable<MemberInfo> accessors = ReadAccessors;
    if (Config.MethodMappingEnabled) accessors = AddMethods(accessors);
    foreach (var member in accessors)
    {
      if (!nameToMember.ContainsKey(member.Name)) nameToMember.Add(member.Name, member);
      if (Config.Postfixes.Count == 0 && Config.Prefixes.Count == 0) continue;
      CheckPrePostfixes(nameToMember, member);
    }

    return nameToMember;
  }

  private IEnumerable<MemberInfo> AddMethods(IEnumerable<MemberInfo> accessors)
  {
    var publicNoArgMethods = GetPublicNoArgMethods();
    var publicNoArgExtensionMethods =
      GetPublicNoArgExtensionMethods(Config.SourceExtensionMethods.Where(Config.ShouldMapMethod));
    return accessors.Concat(publicNoArgMethods).Concat(publicNoArgExtensionMethods);
  }

  private void CheckPrePostfixes(Dictionary<string, MemberInfo> nameToMember, MemberInfo member)
  {
    foreach (var memberName in PossibleNames(member.Name, Config.Prefixes, Config.Postfixes))
      if (!nameToMember.ContainsKey(memberName))
        nameToMember.Add(memberName, member);
  }

  public static IEnumerable<string> PossibleNames(string memberName, List<string> prefixes, List<string> postfixes)
  {
    foreach (var prefix in prefixes)
    {
      if (!memberName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
      var withoutPrefix = memberName.Substring(prefix.Length);
      yield return withoutPrefix;
      foreach (var s in PostFixes(postfixes, withoutPrefix)) yield return s;
    }

    foreach (var s in PostFixes(postfixes, memberName)) yield return s;
  }

  private static IEnumerable<string> PostFixes(List<string> postfixes, string name)
  {
    foreach (var postfix in postfixes)
    {
      if (!name.EndsWith(postfix, StringComparison.OrdinalIgnoreCase)) continue;
      yield return name.Remove(name.Length - postfix.Length);
    }
  }

  private IEnumerable<MethodInfo> GetPublicNoArgExtensionMethods(IEnumerable<MethodInfo> sourceExtensionMethodSearch)
  {
    var explicitExtensionMethods =
      sourceExtensionMethodSearch.Where(method => method.GetParameters()[0].ParameterType.IsAssignableFrom(Type));
    var genericInterfaces = Type.GetInterfacesCache().Where(t => t.IsGenericType);
    if (Type.IsInterface && Type.IsGenericType) genericInterfaces = genericInterfaces.Union(new[] { Type });
    return explicitExtensionMethods.Union
    (
      from genericInterface in genericInterfaces
      let genericInterfaceArguments = genericInterface.GenericTypeArguments
      let matchedMethods = (
        from extensionMethod in sourceExtensionMethodSearch
        where !extensionMethod.IsGenericMethodDefinition
        select extensionMethod
      ).Concat(
        from extensionMethod in sourceExtensionMethodSearch
        where extensionMethod.IsGenericMethodDefinition
              && extensionMethod.GetGenericArguments().Length == genericInterfaceArguments.Length
        select extensionMethod.MakeGenericMethod(genericInterfaceArguments)
      )
      from methodMatch in matchedMethods
      where methodMatch.GetParameters()[0].ParameterType.IsAssignableFrom(genericInterface)
      select methodMatch
    );
  }

  private MemberInfo[] BuildReadAccessors()
  {
    // Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
    IEnumerable<MemberInfo> members = GetProperties(PropertyReadable)
      .GroupBy(x => x.Name) // group properties of the same name together
      .Select(x => x.First());
    if (Config.FieldMappingEnabled) members = members.Concat(GetFields(FieldReadable));
    return members.ToArray();
  }

  private MemberInfo[] BuildWriteAccessors()
  {
    // Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
    IEnumerable<MemberInfo> members = GetProperties(PropertyWritable)
      .GroupBy(x => x.Name) // group properties of the same name together
      .Select(
        x => x.FirstOrDefault(y => y.CanWrite && y.CanRead) ??
             x.First()); // favor the first property that can both read & write - otherwise pick the first one
    if (Config.FieldMappingEnabled) members = members.Concat(GetFields(FieldWritable));
    return members.ToArray();
  }

  private static bool PropertyReadable(PropertyInfo propertyInfo)
  {
    return propertyInfo.CanRead;
  }

  private static bool FieldReadable(FieldInfo fieldInfo)
  {
    return true;
  }

  private static bool PropertyWritable(PropertyInfo propertyInfo)
  {
    return propertyInfo.CanWrite || propertyInfo.PropertyType.IsCollection();
  }

  private static bool FieldWritable(FieldInfo fieldInfo)
  {
    return !fieldInfo.IsInitOnly;
  }

  private IEnumerable<Type> GetTypeInheritance()
  {
    return Type.IsInterface ? new[] { Type }.Concat(Type.GetInterfacesCache()) : Type.GetTypeInheritance();
  }

  private IEnumerable<PropertyInfo> GetProperties(Func<PropertyInfo, bool> propertyAvailableFor)
  {
    return GetTypeInheritance().SelectMany(
      type => type.GetProperties(TypeExtensions.InstanceFlags).Where(
        property => propertyAvailableFor(property) && Config.ShouldMapProperty(property)));
  }

  private IEnumerable<MemberInfo> GetFields(Func<FieldInfo, bool> fieldAvailableFor)
  {
    return GetTypeInheritance().SelectMany(
      type => type.GetFields(TypeExtensions.InstanceFlags)
        .Where(field => fieldAvailableFor(field) && Config.ShouldMapField(field)));
  }

  private IEnumerable<MethodInfo> GetPublicNoArgMethods()
  {
    return Type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(
        m => m.DeclaringType != Metadata<object>.Type && m.ReturnType != Metadata.Void && Config.ShouldMapMethod(m) &&
             m.GetParameters().Length == 0);
  }
}