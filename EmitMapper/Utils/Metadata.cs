using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EmitMapper.Utils;

public class Metadata<T>
{
    public static readonly Type Type = typeof(T);

    public static FieldInfo Field<TX>(Expression<Func<T, TX>> expression)
    {
        return (expression.Body as MemberExpression)?.Member as FieldInfo;
    }

    public static PropertyInfo Property<TX>(Expression<Func<T, TX>> expression)
    {
        return (expression.Body as MemberExpression)?.Member as PropertyInfo;
    }

    public static MethodInfo Method(Expression<Action<T>> expression)
    {
        return (expression.Body as MethodCallExpression)?.Method;
    }

    public static MethodInfo Method<TX>(Expression<Func<T, TX>> expression)
    {
        return (expression.Body as MethodCallExpression)?.Method;
    }
}