using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

public static class ReflectionUtils
{
  public static bool IsNullable(Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
  }

  /// <summary>
  ///   Fixed: Get Full hierarchy with all parent interfaces members.
  /// </summary>
  public static MemberInfo[] GetPublicFieldsAndProperties(Type type)
  {
    var result = type.GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(
      mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field).ToList();

    var interfaces = type.GetInterfaces();
    foreach (var iface in interfaces)
    {
      var ifaceResult = GetPublicFieldsAndProperties(iface);
      result.AddRange(ifaceResult);
    }

    return result.ToArray();
  }

  public static MatchedMember[] GetCommonMembers(Type first, Type second, Func<string, string, bool> matcher)
  {
    matcher ??= (f, s) => f == s;
    var firstMembers = GetPublicFieldsAndProperties(first);
    var secondMembers = GetPublicFieldsAndProperties(first);
    var result = new List<MatchedMember>();
    foreach (var f in firstMembers)
    {
      var s = secondMembers.FirstOrDefault(sm => matcher(f.Name, sm.Name));
      if (s != null)
        result.Add(new MatchedMember(f, s));
    }

    return result.ToArray();
  }

  public static IEnumerable<KeyValuePair<string, Tuple<MemberInfo, Type>>> GetTypeDataContainerDescription(Type to)
  {
    throw new NotImplementedException();
  }

  public static Type GetMemberType(MemberInfo mi)
  {
    return mi switch
    {
      PropertyInfo propertyInfo => propertyInfo.PropertyType,
      FieldInfo fieldInfo => fieldInfo.FieldType,
      MethodInfo methodInfo => methodInfo.ReturnType,
      _ => null
    };
  }

  public static IEnumerable<Tuple<string, Type>> GetDataMemberDefinition(MemberInfo destinationMember)
  {
    throw new NotImplementedException();
  }

  public static bool HasDefaultConstructor(Type type)
  {
    return type.GetConstructor(Type.EmptyTypes) != null;
  }

  public static object ConvertValue(object value, Type fieldType, Type destinationType)
  {
    throw new NotImplementedException();
  }

  public class MatchedMember
  {
    public MatchedMember(MemberInfo first, MemberInfo second)
    {
      First = first;
      Second = second;
    }

    public MemberInfo First { get; set; }

    public MemberInfo Second { get; set; }
  }
}