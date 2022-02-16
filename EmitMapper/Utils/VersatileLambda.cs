namespace EmitMapper.Utils;

using System;
using System.Linq.Expressions;

public class VersatileLambda<T>
  where T : class
{
  private readonly Expression<T> _expression;

  private readonly Lazy<T> _funcLazy;

  public VersatileLambda(Expression<T> expression)
  {
    if (expression == null) throw new ArgumentNullException(nameof(expression));
    _expression = expression;
    _funcLazy = new Lazy<T>(expression.Compile);
  }

  public static implicit operator Expression<T>(VersatileLambda<T> lambda)
  {
    return lambda?._expression;
  }

  public static implicit operator T(VersatileLambda<T> lambda)
  {
    return lambda?._funcLazy.Value;
  }

  public Expression<T> AsExpression()
  {
    return this;
  }

  public T AsLambda()
  {
    return this;
  }
}