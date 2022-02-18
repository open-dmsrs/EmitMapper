namespace EmitMapper.Benchmarks;

using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using AutoMapper;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using EmitMapper.Benchmarks.TestObject;
using EmitMapper.MappingConfiguration;

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]

// [RPlotExporter]
[MemoryDiagnoser]
public class MapperBenchmark
{
  private const int IterationCount = 1_000;

  private IMapper _autoMapper;

  private BenchNestedSource _benchSource;

  private ObjectsMapper<BenchNestedSource, BenchNestedDestination> _benchSourceEmitMapper;

  private List<BenchNestedSource> _benchSources1000List;

  private List<SimpleTypesSource> _simple1000List;

  private List<SimpleTypesSource> _simple100List;

  private ObjectsMapper<SimpleTypesSource, SimpleTypesDestination> _simpleEmitMapper;

  private SimpleTypesSource _simpleSource;

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination BenchNested_a_HardMapper()
  {
    return HardMap(_benchSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination BenchNested_b_EmitMapper()
  {
    return _benchSourceEmitMapper.Map(_benchSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination BenchNested_c_AutoMapper()
  {
    return _autoMapper.Map<BenchNestedSource, BenchNestedDestination>(_benchSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> BenchNested1000_a_HardMapper()
  {
    return _benchSources1000List.Select(s => HardMap(s)).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> BenchNested1000_b_EmitMapper()
  {
    return _benchSourceEmitMapper.MapEnum(_benchSources1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> BenchNested1000_c_AutoMapper()
  {
    return _autoMapper.Map<List<BenchNestedSource>, List<BenchNestedDestination>>(_benchSources1000List);
  }

  [GlobalSetup]
  public void Setup()
  {
    var fixture = new Fixture();
    _benchSourceEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchNestedSource, BenchNestedDestination>();
    _simpleEmitMapper =
      ObjectMapperManager.DefaultInstance.GetMapper<SimpleTypesSource, SimpleTypesDestination>(new DefaultMapConfig());
    var config = new MapperConfiguration(
      cfg =>
        {
          cfg.CreateMap<BenchNestedSource, BenchNestedDestination>();
          cfg.CreateMap<BenchNestedSource.Nested2, BenchNestedDestination.Inner2>();
          cfg.CreateMap<BenchNestedSource.Nested1, BenchNestedDestination.Inner1>();
          cfg.CreateMap<SimpleTypesSource, SimpleTypesDestination>();
        });
    _autoMapper = config.CreateMapper();

    _benchSource = fixture.Create<BenchNestedSource>();
    _simpleSource = fixture.Create<SimpleTypesSource>();
    _simple100List = fixture.CreateMany<SimpleTypesSource>(100).ToList();
    _simple1000List = fixture.CreateMany<SimpleTypesSource>(1000).ToList();
    _benchSources1000List = fixture.CreateMany<BenchNestedSource>(1000).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination SimpleTypes_a_HardMapper()
  {
    return HardMap(_simpleSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination SimpleTypes_b_EmitMapper()
  {
    return _simpleEmitMapper.Map(_simpleSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination SimpleTypes_c_AutoMapper()
  {
    return _autoMapper.Map<SimpleTypesSource, SimpleTypesDestination>(_simpleSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes100_a_HardMapper()
  {
    return _simple100List.Select(s => HardMap(s)).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes100_b_EmitMapper()
  {
    return _simpleEmitMapper.MapEnum(_simple100List).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes100_c_AutoMapper()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple100List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes1000_a_HardMapper()
  {
    return _simple1000List.Select(s => HardMap(s)).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes1000_b_EmitMapper()
  {
    return _simpleEmitMapper.MapEnum(_simple1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes1000_c_AutoMapper()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple1000List);
  }

  public void Usage()
  {
    var simple = ObjectMapperManager.DefaultInstance.GetMapper<BenchNestedSource, BenchNestedDestination>();
    var dest = simple.Map(_benchSource); // for single object;
    var dests = simple.MapEnum(_benchSources1000List); // for list object
  }

  private static BenchNestedDestination.Inner1 HardMap(BenchNestedSource.Nested1 inner)
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

  private static BenchNestedDestination.Inner2 HardMap(BenchNestedSource.Nested2 inner)
  {
    var result = new BenchNestedDestination.Inner2();
    result.I = inner.I;
    result.Str1 = inner.Str1;
    result.Str2 = inner.Str2;
    return result;
  }

  private static BenchNestedDestination HardMap(BenchNestedSource s)
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

  private static SimpleTypesDestination HardMap(SimpleTypesSource s)
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
      result.N5 = decimal.ToInt32(s.N5.Value);
    result.N6 = (int)s.N6;
    result.N7 = s.N7;
    result.N8 = s.N8;
    return result;
  }
}

/*******
 * 
 

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.2037 (1909/November2019Update/19H2)
Intel Core i5-8350U CPU 1.70GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]   : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0

|                       Method |           Mean |       Error |      StdDev |         Median | Ratio |  Gen 0 |  Gen 1 | Allocated |
|----------------------------- |---------------:|------------:|------------:|---------------:|------:|-------:|-------:|----------:|
|     BenchNested_a_HardMapper |      0.8563 ns |   0.0173 ns |   0.0341 ns |      0.8489 ns |  1.00 | 0.0010 |      - |       3 B |
|     BenchNested_b_EmitMapper |      0.8897 ns |   0.0431 ns |   0.1264 ns |      0.8451 ns |  1.00 | 0.0010 |      - |       3 B |
|     BenchNested_c_AutoMapper |     16.5678 ns |   0.7823 ns |   2.2945 ns |     16.7384 ns |  1.00 | 0.0009 |      - |       3 B |
| BenchNested1000_a_HardMapper |  3,988.9549 ns |  79.6474 ns |  97.8142 ns |  3,965.5109 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
| BenchNested1000_b_EmitMapper |  3,781.9851 ns |  74.5756 ns | 122.5298 ns |  3,776.3773 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
| BenchNested1000_c_AutoMapper | 16,978.1985 ns | 333.1485 ns | 396.5896 ns | 16,966.0594 ns |  1.00 | 0.4688 | 0.2188 |   3,033 B |
|     SimpleTypes_a_HardMapper |      0.0310 ns |   0.0008 ns |   0.0023 ns |      0.0307 ns |  1.00 | 0.0000 |      - |         - |
|     SimpleTypes_b_EmitMapper |      0.0512 ns |   0.0009 ns |   0.0010 ns |      0.0513 ns |  1.00 | 0.0000 |      - |         - |
|     SimpleTypes_c_AutoMapper |      0.1718 ns |   0.0035 ns |   0.0076 ns |      0.1709 ns |  1.00 | 0.0000 |      - |         - |
|  SimpleTypes100_a_HardMapper |      4.0774 ns |   0.0804 ns |   0.1714 ns |      4.0509 ns |  1.00 | 0.0041 |      - |      13 B |
|  SimpleTypes100_b_EmitMapper |      8.4392 ns |   0.4806 ns |   1.4020 ns |      8.3280 ns |  1.00 | 0.0044 |      - |      14 B |
|  SimpleTypes100_c_AutoMapper |      8.4149 ns |   0.5269 ns |   1.5285 ns |      8.0763 ns |  1.00 | 0.0045 |      - |      14 B |
| SimpleTypes1000_a_HardMapper |     53.3382 ns |   1.0229 ns |   2.8684 ns |     52.6775 ns |  1.00 | 0.0320 | 0.0103 |     128 B |
| SimpleTypes1000_b_EmitMapper |     81.6188 ns |   1.9400 ns |   5.5036 ns |     79.9001 ns |  1.00 | 0.0308 | 0.0101 |     128 B |
| SimpleTypes1000_c_AutoMapper |     89.3916 ns |   4.3419 ns |  12.6655 ns |     86.0168 ns |  1.00 | 0.0369 | 0.0082 |     137 B |



 * 
 * ******/