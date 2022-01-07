using System;
using System.Diagnostics;
using AutoMapper;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace Benchmarks;

public class CustomizationTest
{
  private static ObjectsMapper<B2, A2> _emitMapper;
  private static IMapper _autoMapper;

  private static long EmitMapper_Custom(int mappingsCount)
  {
    var s = new B2();
    var d = new A2();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = _emitMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  private static long AutoMapper_Custom(int mappingsCount)
  {
    var s = new B2();
    var d = new A2();

    var sw = new Stopwatch();
    sw.Start();
    for (var i = 0; i < mappingsCount; ++i) d = _autoMapper.Map(s, d);
    sw.Stop();
    return sw.ElapsedMilliseconds;
  }

  public static void Initialize()
  {
    _emitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
      new DefaultMapConfig()
        .ConstructBy(() => new A2.Int(0))
        .NullSubstitution<decimal?, decimal>(state => 42)
        .NullSubstitution<bool?, bool>(state => true)
        .NullSubstitution<int?, int>(state => 42)
        .NullSubstitution<long?, long>(state => 42)
        .ConvertUsing<long, int>(value => (int)value + 1)
        .ConvertUsing<short, int>(value => value + 1)
        .ConvertUsing<byte, int>(value => value + 1)
        .ConvertUsing<decimal, int>(value => (int)value + 1)
        .ConvertUsing<float, int>(value => (int)value + 1)
        .ConvertUsing<char, int>(value => value + 1)
    );
    var config = new MapperConfiguration(
      cfg =>
      {
        cfg.CreateMap<A2, B2>();
        cfg.CreateMap<B2.Int, A2.Int>().ConstructUsing(s => new A2.Int(0));

        cfg.CreateMap<long, int>().ConstructUsing(s => (int)s + 1);
        cfg.CreateMap<short, int>().ConstructUsing(s => s + 1);
        cfg.CreateMap<byte, int>().ConstructUsing(s => s + 1);
        cfg.CreateMap<decimal, int>().ConstructUsing(s => (int)s + 1);
        cfg.CreateMap<float, int>().ConstructUsing(s => (int)s + 1);
        cfg.CreateMap<char, int>().ConstructUsing(s => s + 1);

        cfg.CreateMap<B2, A2>()
          .ForMember(s => s.Nullable1, opt => opt.NullSubstitute((decimal)42))
          .ForMember(s => s.Nullable2, opt => opt.NullSubstitute(true))
          .ForMember(s => s.Nullable3, opt => opt.NullSubstitute(42))
          .ForMember(s => s.Nullable4, opt => opt.NullSubstitute((long)42));
      });
    _autoMapper = config.CreateMapper();
  }

  public static void Run()
  {
    var mappingsCount = 1000000;
    Console.WriteLine("Auto Mapper (Custom): {0} milliseconds", AutoMapper_Custom(mappingsCount));
    Console.WriteLine("Emit Mapper (Custom): {0} milliseconds", EmitMapper_Custom(mappingsCount));
  }

  public class A2
  {
    public bool Nullable2;

    public decimal Nullable1;

    public int N1;
    public int N2;
    public int N3;
    public int N4;
    public int N5;
    public int N6;
    public int N7;
    public int N8;
    public int Nullable3;

    public Int I1;
    public Int I2;
    public Int I3;
    public Int I4;
    public Int I5;
    public long Nullable4;

    public string Str1;

    public struct Int
    {
      public int I;

      public Int(int i)
      {
        I = i;
      }
    }
  }

  public class B2
  {
    public bool? Nullable2;
    public byte N4 = 4;
    public char N8 = 'a';
    public decimal N5 = 5;

    public decimal? Nullable1;
    public float N6 = 6;
    public int N1 = 1;
    public int N7 = 7;

    public Int I1 = new(42);
    public Int I2 = new(42);
    public Int I3 = new(42);
    public Int I4 = new(42);
    public Int I5 = new(42);
    public int? Nullable3;
    public long N2 = 2;
    public long? Nullable4;
    public short N3 = 3;

    public string Str1 = "str1";

    public struct Int
    {
      public int I;

      public Int(int i)
      {
        I = i;
      }
    }
  }
}
/*
Auto Mapper (Custom): 50142 milliseconds
Emit Mapper (Custom): 197 milliseconds
*/