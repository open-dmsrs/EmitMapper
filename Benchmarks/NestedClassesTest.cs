using System;
using System.Diagnostics;
using AutoMapper;
using EmitMapper;

namespace Benchmarks;

public class NestedClassesTest
{
  private static ObjectsMapper<BenchSource, BenchDestination> _emitMapper;
  private static IMapper _autoMapper;

  private static long BenchHandwrittenMapper(int mappingsCount)
  {
    var s = new BenchSource();
    var d = new BenchDestination();
    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  private static long BenchEmitMapper(int mappingsCount)
  {
    var s = new BenchSource();
    var d = new BenchDestination();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) _emitMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  private static long BenchAutoMapper(int mappingsCount)
  {
    var s = new BenchSource();
    var d = new BenchDestination();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) _autoMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  public static void Initialize()
  {
    _emitMapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchSource, BenchDestination>();
    var config = new MapperConfiguration(
      cfg =>
      {
        cfg.CreateMap<BenchSource.Int1, BenchDestination.Int1>();
        cfg.CreateMap<BenchSource.Int2, BenchDestination.Int2>();
        cfg.CreateMap<BenchSource, BenchDestination>();
      });
    _autoMapper = config.CreateMapper();
  }

  public static void Run()
  {
    var mappingsCount = 100000;
    Console.WriteLine("Auto Mapper (Nested): {0} milliseconds", BenchAutoMapper(mappingsCount));
    Console.WriteLine("Emit Mapper (Nested): {0} milliseconds", BenchEmitMapper(mappingsCount));
    Console.WriteLine("Handwritten Mapper (Nested): {0} milliseconds", BenchHandwrittenMapper(mappingsCount));
  }

  public class BenchSource
  {
    public byte N4;

    public int N2;
    public int N7;
    public int N8;
    public int N9;

    public Int2 I1 = new();
    public Int2 I2 = new();
    public Int2 I3 = new();
    public Int2 I4 = new();
    public Int2 I5 = new();
    public Int2 I6 = new();
    public Int2 I7 = new();
    public Int2 I8 = new();
    public long N3;
    public short N5;

    public string S1 = "1";
    public string S2 = "2";
    public string S3 = "3";
    public string S4 = "4";
    public string S5 = "5";
    public string S6 = "6";
    public string S7 = "7";
    public uint N6;

    public class Int1
    {
      public int I = 10;
      public string Str1 = "1";
      public string Str2 = null;
    }

    public class Int2
    {
      public Int1 I1 = new();
      public Int1 I2 = new();
      public Int1 I3 = new();
      public Int1 I4 = new();
      public Int1 I5 = new();
      public Int1 I6 = new();
      public Int1 I7 = new();
    }
  }

  public class BenchDestination
  {
    public long N2 = 2;
    public long N3 = 3;
    public long N4 = 4;
    public long N5 = 5;
    public long N6 = 6;
    public long N7 = 7;
    public long N8 = 8;
    public long N9 = 9;

    public string S1;
    public string S2;
    public string S3;
    public string S4;
    public string S5;
    public string S6;
    public string S7;

    public Int2 I1 { get; set; }
    public Int2 I2 { get; set; }
    public Int2 I3 { get; set; }
    public Int2 I4 { get; set; }
    public Int2 I5 { get; set; }
    public Int2 I6 { get; set; }
    public Int2 I7 { get; set; }
    public Int2 I8 { get; set; }

    public class Int1
    {
      public int I;
      public string Str1;
      public string Str2;
    }

    public class Int2
    {
      public Int1 I1;
      public Int1 I2;
      public Int1 I3;
      public Int1 I4;
      public Int1 I5;
      public Int1 I6;
      public Int1 I7;
    }
  }


  #region Hndwritten mapper

  private static BenchDestination.Int1 Map(BenchSource.Int1 s, BenchDestination.Int1 d)
  {
    if (s == null) return null;
    if (d == null) d = new BenchDestination.Int1();
    d.I = s.I;
    d.Str1 = s.Str1;
    d.Str2 = s.Str2;
    return d;
  }

  private static BenchDestination.Int2 Map(BenchSource.Int2 s, BenchDestination.Int2 d)
  {
    if (s == null) return null;

    if (d == null) d = new BenchDestination.Int2();
    d.I1 = Map(s.I1, d.I1);
    d.I2 = Map(s.I2, d.I2);
    d.I3 = Map(s.I3, d.I3);
    d.I4 = Map(s.I4, d.I4);
    d.I5 = Map(s.I5, d.I5);
    d.I6 = Map(s.I6, d.I6);
    d.I7 = Map(s.I7, d.I7);

    return d;
  }

  private static BenchDestination Map(BenchSource s, BenchDestination d)
  {
    if (s == null) return null;
    if (d == null) d = new BenchDestination();
    d.I1 = Map(s.I1, d.I1);
    d.I2 = Map(s.I2, d.I2);
    d.I3 = Map(s.I3, d.I3);
    d.I4 = Map(s.I4, d.I4);
    d.I5 = Map(s.I5, d.I5);
    d.I6 = Map(s.I6, d.I6);
    d.I7 = Map(s.I7, d.I7);
    d.I8 = Map(s.I8, d.I8);

    d.N2 = s.N2;
    d.N3 = s.N3;
    d.N4 = s.N4;
    d.N5 = s.N5;
    d.N6 = s.N6;
    d.N7 = s.N7;
    d.N8 = s.N8;
    d.N9 = s.N9;

    d.S1 = s.S1;
    d.S2 = s.S2;
    d.S3 = s.S3;
    d.S4 = s.S4;
    d.S5 = s.S5;
    d.S6 = s.S6;
    d.S7 = s.S7;

    return d;
  }

  #endregion
}

/*
Auto Mapper (simple): 52238 milliseconds
Emit Mapper (simple): 102 milliseconds
Handwritten Mapper (simple): 97 milliseconds
*/