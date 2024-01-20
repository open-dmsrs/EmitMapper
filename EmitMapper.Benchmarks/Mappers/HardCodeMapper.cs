namespace EmitMapper.Benchmarks.Mappers;

  /// <summary>
  /// The hard code mapper.
  /// </summary>
  [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
  [KurtosisColumn]
  [MediumRunJob]
  [MemoryDiagnoser]
  [SkewnessColumn]
  public static class HardCodeMapper
  {
    /// <summary>
    /// Hards the map.
    /// </summary>
    /// <param name="inner">The inner.</param>
    /// <returns>A BenchNestedDestination.Inner1.</returns>
    public static BenchNestedDestination.Inner1 HardMap(BenchNestedSource.Nested1 inner)
    {
      var result = new BenchNestedDestination.Inner1();
      result.I1 = HardMap(inner.I1);
      result.I2 = HardMap(inner.I2);
      result.I3 = HardMap(inner.I3);
      result.I4 = HardMap(inner.I4);
      result.I5 = HardMap(inner.I5);
      result.I6 = HardMap(inner.I6);
      result.I7 = HardMap(inner.I7);

      return result;
    }

    /// <summary>
    /// Hards the map.
    /// </summary>
    /// <param name="inner">The inner.</param>
    /// <returns>A BenchNestedDestination.Inner2.</returns>
    public static BenchNestedDestination.Inner2 HardMap(BenchNestedSource.Nested2 inner)
    {
      var result = new BenchNestedDestination.Inner2();
      result.I = inner.I;
      result.Str1 = inner.Str1;
      result.Str2 = inner.Str2;

      return result;
    }

    /// <summary>
    /// Hards the map.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>A BenchNestedDestination.</returns>
    public static BenchNestedDestination HardMap(BenchNestedSource s)
    {
      BenchNestedDestination result = new();
      result.I1 = HardMap(s.I1);

      result.I2 = HardMap(s.I2);
      result.I3 = HardMap(s.I3);
      result.I4 = HardMap(s.I4);
      result.I5 = HardMap(s.I5);
      result.I6 = HardMap(s.I6);
      result.I7 = HardMap(s.I7);
      result.I8 = HardMap(s.I8);
      result.N2 = s.N2;
      result.N3 = s.N3;
      result.N4 = s.N4;
      result.N5 = s.N5;
      result.N6 = s.N6;
      result.N7 = s.N7;
      result.N8 = s.N8;
      result.N9 = s.N9;
      result.S1 = s.S1;
      result.S2 = s.S2;
      result.S3 = s.S3;
      result.S4 = s.S4;
      result.S5 = s.S5;
      result.S6 = s.S6;
      result.S7 = s.S7;

      return result;
    }

    /// <summary>
    /// Hards the map.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>A SimpleTypesDestination.</returns>
    public static SimpleTypesDestination HardMap(SimpleTypesSource s)
    {
      SimpleTypesDestination result = new();
      result.Str1 = s.Str1;
      result.Str2 = s.Str2;
      result.Str3 = s.Str3;
      result.Str4 = s.Str4;
      result.Str5 = s.Str5;
      result.Str6 = s.Str6;
      result.Str7 = s.Str7;
      result.Str8 = s.Str8;
      result.Str9 = s.Str9;

      result.N1 = s.N1;
      result.N2 = (int)s.N2;
      result.N3 = s.N3;
      result.N4 = s.N4;

      if (s.N5.HasValue)
      {
        result.N5 = decimal.ToInt32(s.N5.Value);
      }

      result.N6 = (int)s.N6;
      result.N7 = s.N7;
      result.N8 = s.N8;

      return result;
    }
  }