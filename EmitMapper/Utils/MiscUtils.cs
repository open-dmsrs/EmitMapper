namespace EmitMapper.Utils;

using System.Collections.Generic;

internal static class MiscUtils
{
  public static string ToCsv<T>(this IEnumerable<T> collection, string delim)
  {
    if (collection == null)
      return string.Empty;

    return string.Join(delim, collection);
  }
}