using System;
using System.Linq;
using System.Reflection;

namespace EmitMapper
{
    public static class Extensions
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
            Type[] types,object c)
        {
            // todo: need to complete
            throw new NotImplementedException();
           // return type.GetTypeInfo().GetConstructors(a).Where(x => x.GetGenericArguments()==types);

        }
        
        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetField(name);
        }
        public static FieldInfo GetField(this Type type, string name, BindingFlags bfs)
        {
            return type.GetTypeInfo().GetField(name, bfs);
        }

        public static MemberInfo[] GetMembers(this Type type)
        {
            return type.GetTypeInfo().GetMembers();
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

    public class Class1
    {
    }
}
