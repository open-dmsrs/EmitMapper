using System;
using System.Diagnostics;
using AutoMapper;
using EmitMapper;

namespace Benchmarks;

public static class SimpleTest
{
  private static ObjectsMapper<B2, A2> _emitMapper;
  private static IMapper _autoMapper;

  private static A2 HandwrittenMap(B2 s, A2 result)
  {
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
    result.N5 = (int)s.N5;
    result.N6 = (int)s.N6;
    result.N7 = s.N7;
    result.N8 = s.N8;

    return result;
  }

  private static long EmitMapper_Simple(int mappingsCount)
  {
    var s = new B2();
    var d = new A2();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = _emitMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  private static long AutoMapper_Simple(int mappingsCount)
  {
    var s = new B2();
    var d = new A2();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = _autoMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  private static long HandWtitten_Simple(int mappingsCount)
  {
    var s = new B2();
    var d = new A2();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = HandwrittenMap(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }


  public static void Initialize()
  {
    _emitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>();
    _autoMapper = new MapperConfiguration(
      cfg =>
      {
        cfg.CreateMap<B2, A2>();
        cfg.CreateMap<char, int>();
      }).CreateMapper();
  }

  public static void Run()
  {
    var mappingsCount = 1000000;
    Console.WriteLine("Auto Mapper (simple): {0} milliseconds", AutoMapper_Simple(mappingsCount));
    Console.WriteLine("Emit Mapper (simple): {0} milliseconds", EmitMapper_Simple(mappingsCount));
    Console.WriteLine("Handwritten Mapper (simple): {0} milliseconds", HandWtitten_Simple(mappingsCount));
  }

  public class A2
  {
    public int N1;
    public int N2;
    public int N3;
    public int N4;
    public int N5;
    public int N6;
    public int N7;
    public int N8;
    public string Str1;
    public string Str2;
    public string Str3;
    public string Str4;
    public string Str5;
    public string Str6;
    public string Str7;
    public string Str8;
    public string Str9;
  }

  public class B2
  {
    public byte N4 = 4;
    public char N8 = 'a';
    public decimal N5 = 5;
    public float N6 = 6;

    public int N1 = 1;
    public int N7 = 7;
    public long N2 = 2;
    public short N3 = 3;
    public string Str1 = "str1";
    public string Str2 = "str2";
    public string Str3 = "str3";
    public string Str4 = "str4";
    public string Str5 = "str5";
    public string Str6 = "str6";
    public string Str7 = "str7";
    public string Str8 = "str8";
    public string Str9 = "str9";
  }
}

/*
Auto Mapper (simple): 36809 milliseconds
BLToolkit (simple): 34533 milliseconds
Emit Mapper (simple): 117 milliseconds
Handwritten Mapper (simple): 37 milliseconds
*/