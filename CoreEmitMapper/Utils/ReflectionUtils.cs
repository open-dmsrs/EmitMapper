using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils
{
    public class ReflectionUtils
    {
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }


        /// Get Full hierarchy with all parent interfaces members.
        public static MemberInfo[] GetPublicFieldsAndProperties(Type type)
        {
            var result = type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
                .ToList();

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
            if (matcher == null)
            {
                matcher = (f, s) => f == s;
            }
            var firstMembers = GetPublicFieldsAndProperties(first);
            var secondMembers = GetPublicFieldsAndProperties(first);
            var result = new List<MatchedMember>();
            foreach (var f in firstMembers)
            {
                var s = secondMembers.FirstOrDefault(sm => matcher(f.Name, sm.Name));
                if (s != null)
                {
                    result.Add(new MatchedMember(f, s));
                }
            }
            return result.ToArray();
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            if (mi is PropertyInfo)
            {
                return ((PropertyInfo) mi).PropertyType;
            }
            if (mi is FieldInfo)
            {
                return ((FieldInfo) mi).FieldType;
            }
            if (mi is MethodInfo)
            {
                return ((MethodInfo) mi).ReturnType;
            }
            return null;
        }

        public static bool HasDefaultConstructor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
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
}