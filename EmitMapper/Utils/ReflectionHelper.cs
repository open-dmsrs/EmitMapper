using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EmitMapper.Utils;

public static class ReflectionHelper
{
    public static Type GetElementType(Type type)
    {
        return type.IsArray ? type.GetElementType() : GetEnumerableElementType(type);
    }

    public static Type GetEnumerableElementType(Type type)
    {
        return type.GetIEnumerableType()?.GenericTypeArguments[0] ?? typeof(object);
    }

    public static bool IsPublic(this PropertyInfo propertyInfo)
    {
        return (propertyInfo.GetGetMethod() ?? propertyInfo.GetSetMethod()) != null;
    }

    public static bool Has<TAttribute>(this MemberInfo member) where TAttribute : Attribute
    {
        return member.IsDefined(typeof(TAttribute));
    }

    public static bool CanBeSet(this MemberInfo member)
    {
        return member is PropertyInfo property ? property.CanWrite : !((FieldInfo)member).IsInitOnly;
    }

    public static Expression GetDefaultValue(this ParameterInfo parameter)
    {
        return parameter is { DefaultValue: null, ParameterType: { IsValueType: true } type }
            ? Expression.Default(type)
            : Expression.Constant(parameter.DefaultValue);
    }

    public static void SetMemberValue(this MemberInfo propertyOrField, object target, object value)
    {
        if (propertyOrField is PropertyInfo property)
        {
            property.SetValue(target, value, null);
            return;
        }

        if (propertyOrField is FieldInfo field)
        {
            field.SetValue(target, value);
            return;
        }

        throw Expected(propertyOrField);
    }

    private static ArgumentOutOfRangeException Expected(MemberInfo propertyOrField)
    {
        return new(nameof(propertyOrField), "Expected a property or field, not " + propertyOrField);
    }

    public static object GetMemberValue(this MemberInfo propertyOrField, object target)
    {
        return propertyOrField switch
        {
            PropertyInfo property => property.GetValue(target, null),
            FieldInfo field => field.GetValue(target),
            _ => throw Expected(propertyOrField)
        };
    }

    public static MemberInfo[] GetMemberPath(Type type, string fullMemberName)
    {
        var memberNames = fullMemberName.Split('.');
        var members = new MemberInfo[memberNames.Length];
        var previousType = type;
        for (var index = 0; index < memberNames.Length; index++)
        {
            var currentType = GetCurrentType(previousType);
            var memberName = memberNames[index];
            var property = currentType.GetInheritedProperty(memberName);
            if (property != null)
            {
                previousType = property.PropertyType;
                members[index] = property;
            }
            else if (currentType.GetInheritedField(memberName) is FieldInfo field)
            {
                previousType = field.FieldType;
                members[index] = field;
            }
            else
            {
                var method = currentType.GetInheritedMethod(memberName);
                previousType = method.ReturnType;
                members[index] = method;
            }
        }

        return members;

        static Type GetCurrentType(Type type)
        {
            return type.IsGenericType && type.IsCollection() ? type.GenericTypeArguments[0] : type;
        }
    }

    public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
    {
        var expressionToCheck = lambdaExpression.Body;
        while (true)
            switch (expressionToCheck)
            {
                case MemberExpression
                {
                    Member: var member, Expression: { NodeType: ExpressionType.Parameter or ExpressionType.Convert }
                }:
                    return member;
                case UnaryExpression { Operand: var operand }:
                    expressionToCheck = operand;
                    break;
                default:
                    throw new ArgumentException(
                        $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                        nameof(lambdaExpression));
            }
    }

    public static Type GetMemberType(this MemberInfo member)
    {
        return member switch
        {
            PropertyInfo property => property.PropertyType,
            MethodInfo method => method.ReturnType,
            FieldInfo field => field.FieldType,
            null => throw new ArgumentNullException(nameof(member)),
            _ => throw new ArgumentOutOfRangeException(nameof(member))
        };
    }
}