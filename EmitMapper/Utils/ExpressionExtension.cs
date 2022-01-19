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

  public static Expression<Func<TFrom, TTo>> Chain<TFrom, TMiddle, TTo>(
    this Expression<Func<TFrom, TMiddle>> first,
    Expression<Func<TMiddle, TTo>> second
  )
  {
    return Lambda<Func<TFrom, TTo>>(
      new SwapVisitor(second.Parameters[0], first.Body).Visit(second.Body),
      first.Parameters
    );
  }

  // this method thanks to Marc Gravell   
  private class SwapVisitor : ExpressionVisitor
  {
    private readonly Expression _from;
    private readonly Expression _to;

    public SwapVisitor(Expression from, Expression to)
    {
      _from = from;
      _to = to;
    }

    public override Expression Visit(Expression node)
    {
      return node == _from ? _to : base.Visit(node);
    }
  }
}