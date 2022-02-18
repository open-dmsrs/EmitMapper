using System;
using System.Collections.Generic;
using System.Linq;

namespace SamplesTests;

public static class TestUtils
{
  public static bool AreEqual<T>(IEnumerable<T> arr1, IEnumerable<T> arr2)
    where T : IEquatable<T>
  {
    if (arr1 is ICollection<T> list1 && arr2 is ICollection<T> list2)
      return list1.Count == list2.Count && list1.All(list2.Contains);

    if (arr1 == null || arr2 == null)
      return arr1 == null && arr2 == null;

    using IEnumerator<T> er1 = arr1.GetEnumerator(), er2 = arr2.GetEnumerator();
    while (er1.MoveNext() && er2.MoveNext())
      if (!Equals(er1.Current, er2.Current))
        return false;

    return true;
  }

  public static bool AreEqual<T>(ICollection<T> arr1, ICollection<T> arr2)
    where T : IEquatable<T>
  {
    if (arr1 == null || arr2 == null)
      return arr1 == null && arr2 == null;

    return arr1.Count == arr2.Count && arr1.All(arr2.Contains);
  }
}