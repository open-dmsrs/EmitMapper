namespace EmitMapper.Tests;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Shouldly;

internal static class CustomAssert
{
  public static void AreEqual(ICollection expected, ICollection actual)
  {
    expected.Count.ShouldBe(actual.Count);
    var enumExpected = expected.GetEnumerator();
    var enumActual = actual.GetEnumerator();
    while (enumExpected.MoveNext() && enumActual.MoveNext())
      enumExpected.Current.ShouldBe(enumActual.Current);
  }

  public static void AreEqualEnum<T>(IEnumerable<T> expected, IEnumerable<T> actual)
  {
    actual.Count().ShouldBe(expected.Count());
    IEnumerator enumExpected = expected.GetEnumerator();
    IEnumerator enumActual = actual.GetEnumerator();
    while (enumExpected.MoveNext() && enumActual.MoveNext())
      enumExpected.Current.ShouldBe(enumActual.Current);
  }
}