using System.Linq.Expressions;

namespace EmitMapper.Utils;

using static Expression;

/// <summary>
/// The expression helper.
/// </summary>
public static class ExpressionHelper
{
  /// <summary>
  /// Chains the Expression.
  /// </summary>
  /// <typeparam name="TFrom"></typeparam>
  /// <typeparam name="TMiddle"></typeparam>
  /// <typeparam name="TTo"></typeparam>
  /// <param name="first">The first.</param>
  /// <param name="second">The second.</param>
  /// <returns><![CDATA[Expression<Func<TFrom, TTo>>]]></returns>
  public static Expression<Func<TFrom, TTo>> Chain<TFrom, TMiddle, TTo>(
    this Expression<Func<TFrom, TMiddle>> first,
    Expression<Func<TMiddle, TTo>> second)
  {
    return Lambda<Func<TFrom, TTo>>(
      new SwapVisitor(second.Parameters[0], first.Body).Visit(second.Body),
      first.Parameters);
  }

  /// <summary>
  /// Tos the object.
  /// </summary>
  /// <param name="expression">The expression.</param>
  /// <returns>An Expression.</returns>
  public static Expression ToObject(this Expression expression)
  {
    return expression.Type.IsValueType ? Convert(expression, Metadata<object>.Type) : expression;
  }

  /// <summary>
  /// Tos the type.
  /// </summary>
  /// <param name="expression">The expression.</param>
  /// <param name="type">The type.</param>
  /// <returns>An Expression.</returns>
  public static Expression ToType(Expression expression, Type type)
  {
    return expression.Type == type ? expression : Convert(expression, type);
  }

  // this method thanks to Marc Gravell
  /// <summary>
  /// The swap visitor.
  /// </summary>
  private class SwapVisitor : ExpressionVisitor
  {
    private readonly Expression _from;

    private readonly Expression _to;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwapVisitor"/> class.
    /// </summary>
    /// <param name="from">The from.</param>
    /// <param name="to">The to.</param>
    public SwapVisitor(Expression from, Expression to)
    {
      _from = from;
      _to = to;
    }

    /// <summary>
    /// Visits the <see cref="Expression"/>.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>An Expression.</returns>
    public override Expression Visit(Expression node)
    {
      return node == _from ? _to : base.Visit(node);
    }
  }
}