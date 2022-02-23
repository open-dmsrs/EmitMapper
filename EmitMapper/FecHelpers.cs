using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace EmitMapper;

internal static class FecHelpers
{
  public static int GetFirstIndex<T>(this IReadOnlyList<T> source, T item)
  {
    if (source.Count != 0)
      for (var i = 0; i < source.Count; ++i)
        if (ReferenceEquals(source[i], item))
          return i;

    return -1;
  }

  [MethodImpl((MethodImplOptions)256)]
  public static T GetArgument<T>(this IReadOnlyList<T> source, int index)
  {
    return source[index];
  }

  [MethodImpl((MethodImplOptions)256)]
  public static ParameterExpression GetParameter(this IReadOnlyList<ParameterExpression> source, int index)
  {
    return source[index];
  }

#if LIGHT_EXPRESSION
        public static IReadOnlyList<PE> ToReadOnlyList(this IParameterProvider source) 
        {
            var count = source.ParameterCount;
            var ps = new ParameterExpression[count];
            for (var i = 0; i < count; ++i)
                ps[i] = source.GetParameter(i);
            return ps;
        }
#else
  public static IReadOnlyList<ParameterExpression> ToReadOnlyList(this IReadOnlyList<ParameterExpression> source)
  {
    return source;
  }
#endif
}