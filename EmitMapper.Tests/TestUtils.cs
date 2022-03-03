using System.Collections.Generic;
using System.Linq;

namespace EmitMapper.Tests;

/// <summary>
///   The test utils.
/// </summary>
public static class TestUtils
{
  /// <summary>
  ///   Are equal.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="arr1">The arr1.</param>
  /// <param name="arr2">The arr2.</param>
  /// <returns>A bool.</returns>
  public static bool AreEqual<T>(IEnumerable<T> arr1, IEnumerable<T> arr2)
    where T : IEqualityComparer<T>
  {
    if (arr1 == null || arr2 == null)
      return arr1 == null && arr2 == null;

    if (arr1 is ICollection<T> list1 && arr2 is ICollection<T> list2)
      return list1.Count == list2.Count && list1.All(list2.Contains);

    using IEnumerator<T> er1 = arr1.GetEnumerator(), er2 = arr2.GetEnumerator();

    while (er1.MoveNext() && er2.MoveNext())
      if (!Equals(er1.Current, er2.Current))
        return false;

    return true;
  }
}