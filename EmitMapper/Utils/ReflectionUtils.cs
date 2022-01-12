using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

public static class ReflectionUtils
{
    private static readonly LazyConcurrentDictionary<MemberInfo, Type> MemberInfoReturnTypes = new();

    public static bool IsNullable(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    ///     Fixed: Get Full hierarchy with all parent interfaces members.
    /// </summary>
    public static IEnumerable<MemberInfo> GetPublicFieldsAndProperties(Type type)
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

    public static Type GetMemberReturnType(MemberInfo mi)
    {
        return MemberInfoReturnTypes.GetOrAdd(
            mi,
            key => key switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                MethodInfo methodInfo => methodInfo.ReturnType,

                _ => null
            });
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

    /// <summary>
    ///     获取源的数据类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static IEnumerable<MemberInfo> GetSourceMembers(Type t)
    {
        return GetMembers(t)
            .Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property or MemberTypes.Method);
    }

    /// <summary>
    ///     根据类型获取成员信息(字段与属性)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberInfo> GetDestinationMembers(Type t)
    {
        return GetMembers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);
    }

    /// <summary>
    ///     根据类型获取成员信息(字段、属性、方法)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberInfo> GetMembers(Type t)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
        return t.GetMembers(bindingFlags);
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