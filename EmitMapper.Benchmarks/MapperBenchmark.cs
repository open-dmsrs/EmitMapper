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
  public BenchNestedDestination AutoMapper_BenchSource()
  {
    return _autoMapper.Map<BenchNestedSource, BenchNestedDestination>(_benchSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> AutoMapper_BenchSourceList1000()
  {
    return _autoMapper.Map<List<BenchNestedSource>, List<BenchNestedDestination>>(_benchSources1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination AutoMapper_Simple()
  {
    return _autoMapper.Map<SimpleTypesSource, SimpleTypesDestination>(_simpleSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> AutoMapper_SimpleList100()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple100List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> AutoMapper_SimpleList1000()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination EmitMapper_BenchSource()
  {
    return _benchSourceEmitMapper.Map(_benchSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> EmitMapper_BenchSourceList1000()
  {
    return _benchSourceEmitMapper.MapEnum(_benchSources1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination EmitMapper_Simple()
  {
    return _simpleEmitMapper.Map(_simpleSource);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> EmitMapper_SimpleList100()
  {
    return _simpleEmitMapper.MapEnum(_simple100List).ToList();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> EmitMapper_SimpleList1000()
  {
    return _simpleEmitMapper.MapEnum(_simple1000List);
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

  private static SimpleTypesDestination HandwrittenMap(SimpleTypesSource s, SimpleTypesDestination result)
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
    if (s.N5.HasValue)
      result.N5 = decimal.ToInt32(s.N5.Value);
    result.N6 = (int)s.N6;
    result.N7 = s.N7;
    result.N8 = s.N8;
    return result;
  }
}

/*
* Summary *
 * Feb 8, 2022
 * The newest benchmark test is shown that the performance problem has been resolved. to see the method ToInt32(decimal?) of class NullableConverter
 BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.2037 (1909/November2019Update/19H2)
   Intel Core i5-8350U CPU 1.70GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
   .NET SDK=6.0.101
   [Host]   : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
   .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
   
   Job=.NET 6.0  Runtime=.NET 6.0
   
   |                         Method |        Mean |     Error |    StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
   |------------------------------- |------------:|----------:|----------:|------:|-------:|-------:|----------:|
   |      EmitMapper_SimpleList1000 |    102.6 ns |   2.17 ns |   5.99 ns |  1.00 | 0.0306 | 0.0101 |     128 B |
   |                                |             |           |           |       |        |        |           |
   |      AutoMapper_SimpleList1000 |    120.8 ns |   5.34 ns |  15.14 ns |  1.00 | 0.0359 | 0.0098 |     137 B |
   |                                |             |           |           |       |        |        |           |
   | EmitMapper_BenchSourceList1000 |  4,607.4 ns | 106.53 ns | 298.73 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
   |                                |             |           |           |       |        |        |           |
   | AutoMapper_BenchSourceList1000 | 16,540.4 ns | 305.95 ns | 603.91 ns |  1.00 | 0.4688 | 0.2344 |   3,033 B |


  BenchmarkDotNet = v0.13.1, OS=Windows 10.0.18363.1977 (1909/November2019Update/19H2)
Intel Core i5-8350U CPU 1.70GHz(Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
  .NET SDK= 6.0.101

  [Host]   : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

  Job=.NET 6.0  Runtime=.NET 6.0

  |                         Method |         Mean |      Error |     StdDev |       Median | Ratio |  Gen 0 |  Gen 1 | Allocated |
  |------------------------------- |-------------:|-----------:|-----------:|-------------:|------:|-------:|-------:|----------:|
  |      EmitMapper_SimpleList1000 |     77.40 ns |   1.538 ns |   3.344 ns |     76.89 ns |  1.00 | 0.0305 | 0.0101 |     128 B |
  |                                |              |            |            |              |       |        |        |           |
  |      AutoMapper_SimpleList1000 |     60.30 ns |   1.201 ns |   2.103 ns |     59.26 ns |  1.00 | 0.0363 | 0.0110 |     137 B |
  |                                |              |            |            |              |       |        |        |           |
  | EmitMapper_BenchSourceList1000 |  3,261.09 ns |  16.168 ns |  14.333 ns |  3,261.93 ns |  1.00 | 0.4805 | 0.2383 |   3,024 B |
  |                                |              |            |            |              |       |        |        |           |
  | AutoMapper_BenchSourceList1000 | 14,335.81 ns | 208.166 ns | 311.573 ns | 14,256.34 ns |  1.00 | 0.4688 | 0.2188 |   3,033 B |

  // * Summary *

  BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1499 (21H2)
  Intel Core i7-3740QM CPU 2.70GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
  .NET SDK=6.0.101
    [Host]   : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
    .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

  Job=.NET 6.0  Runtime=.NET 6.0

  |                         Method |         Mean |      Error |     StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
  |------------------------------- |-------------:|-----------:|-----------:|------:|-------:|-------:|----------:|
  |      EmitMapper_SimpleList1000 |    126.07 ns |   2.281 ns |   2.134 ns |  1.00 | 0.0305 | 0.0100 |     128 B |
  |                                |              |            |            |       |        |        |           |
  |      AutoMapper_SimpleList1000 |     90.71 ns |   0.934 ns |   0.874 ns |  1.00 | 0.0361 | 0.0089 |     137 B |
  |                                |              |            |            |       |        |        |           |
  | EmitMapper_BenchSourceList1000 |  4,469.99 ns |  87.383 ns |  81.738 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
  |                                |              |            |            |       |        |        |           |
  | AutoMapper_BenchSourceList1000 | 17,409.99 ns | 260.764 ns | 231.160 ns |  1.00 | 0.4688 | 0.2188 |   3,033 B |

*/