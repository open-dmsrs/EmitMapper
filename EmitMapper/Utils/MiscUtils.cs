namespace EmitMapper.Utils;

using System.Collections.Generic;
using System.Text;

internal static class MiscUtils
{
    public static string ToCsv<T>(this IEnumerable<T> collection, string delim)
    {
        if (collection == null)
            return "";

        var result = new StringBuilder();
        foreach (var value in collection)
        {
            result.Append(value);
            result.Append(delim);
        }

        if (result.Length > 0)
            result.Length -= delim.Length;
        return result.ToString();
    }
}