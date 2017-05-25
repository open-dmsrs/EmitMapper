using System;
using System.Reflection;
using System.Linq;
#if !UnitTest
namespace EmitMapper
#else
namespace EmitMapperTests
#endif
{
    public static class TypeExtensions
    {
        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return type.GetTypeInfo().GetMethod(methodName);
        }
        public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags flags)
        {
            return type.GetTypeInfo().GetMethod(methodName, flags);
        }
        public static MethodInfo GetMethod(this Type type, string methodName, Type[] types)
        {
            return type.GetTypeInfo().GetMethod(methodName, types);
        }
        public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
        {
            return type.GetTypeInfo().GetMethods(flags);
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return type.GetTypeInfo().GetProperty(propertyName);
        }
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type.GetTypeInfo().GetConstructor(types);

        }
        public static ConstructorInfo GetConstructor(this Type type, int a, Type[] types)
        {
            return type.GetTypeInfo().GetConstructor(types);

        }
        public static ConstructorInfo GetConstructor(this Type type, BindingFlags a, object b,
            Type[] types, object c)
        {
            // todo: need to complete
            // throw new NotImplementedException();
            return type.GetTypeInfo().GetConstructor(types);//.FirstOrDefault(x => true);

        }

        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetField(name);
        }
        public static FieldInfo GetField(this Type type, string name, BindingFlags bfs)
        {
            return type.GetTypeInfo().GetField(name, bfs);
        }
        public static FieldInfo[] GetFields(this Type type)
        {
            return type.GetTypeInfo().GetFields();
        }
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().GetProperties();
        }
        public static PropertyInfo[] GetCustomAttributes(this PropertyInfo pi, Type attributeType, bool isInherit)
        {
            return pi.GetCustomAttributes(attributeType, isInherit);
        }

        public static PropertyInfo[] GetCustomAttributes(this FieldInfo fi, Type attributeType, bool isInherit)
        {
            return fi.GetCustomAttributes(attributeType, isInherit);
        }
        public static MemberInfo[] GetMembers(this Type type)
        {
            return type.GetTypeInfo().GetMembers();
        }


        public static MemberInfo[] GetMember(this Type type, string name)
        {
            return type.GetTypeInfo().GetMember(name);
        }

        public static MemberInfo[] GetMembers(this Type type, BindingFlags bfs)
        {
            return type.GetTypeInfo().GetMembers(bfs);
        }
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GetGenericArguments();
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c);
        }
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }


    }
}