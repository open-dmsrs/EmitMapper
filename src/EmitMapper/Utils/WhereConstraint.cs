using System;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

public class WhereConstraint<T> : VersatileLambda<Func<T, bool>>
{
  public WhereConstraint(Expression<Func<T, bool>> lambda)
    : base(lambda)
  {
  }
}