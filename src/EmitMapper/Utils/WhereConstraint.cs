using System;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

/// <summary>
///   The where constraint.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WhereConstraint<T> : VersatileLambda<Func<T, bool>>
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="WhereConstraint&lt;T&gt;" /> class.
  /// </summary>
  /// <param name="lambda">The lambda.</param>
  public WhereConstraint(Expression<Func<T, bool>> lambda)
    : base(lambda)
  {
  }
}