using System;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

using static Expression;

public static class ExpressionExtension
{
    public static Expression ToObject(this Expression expression)
    {
        return expression.Type.IsValueType ? Convert(expression, Metadata<object>.Type) : expression;
    }

    public static Expression ToType(Expression expression, Type type)
    {
        return expression.Type == type ? expression : Convert(expression, type);
    }
}