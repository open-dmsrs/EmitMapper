using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EmitMapper.Benchmarks.TestData;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
//[RPlotExporter]
[MemoryDiagnoser]
public class MapperBenchmark
{
  private const int IterationCount = 1_000;
  private B2 _simpleSource;

  private BenchSource _benchSource;
  private IMapper _autoMapper;
  private List<B2> _simple1000List;

  private List<B2> _simple100List;
  private List<BenchSource> _benchSources1000List;
  private ObjectsMapper<B2, A2> _simpleEmitMapper;

  private ObjectsMapper<BenchSource, BenchDestination> _benchSourceEmitMapper;

  [GlobalSetup]
  public void Setup()
  {
    //Console.WriteLine($"Current:{DateTime.Now.Ticks}");

    var fixture = new Fixture();
    _benchSourceEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchSource, BenchDestination>();
    _simpleEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
      new DefaultMapConfig()
        .ConstructBy(() => new A2())
        .NullSubstitution<decimal?, int>(state => 0)
        .NullSubstitution<bool?, bool>(state => true)
        .NullSubstitution<int?, int>(state => 42)
        .NullSubstitution<long?, long>(state => 42)
        .ConvertUsing<long, int>(value => (int)value)
        .ConvertUsing<short, int>(value => value)
        .ConvertUsing<byte, int>(value => value)
        //.ConvertUsing<decimal, int>(value => (int)value + 1)
        .ConvertUsing<float, int>(value => (int)value)
        .ConvertUsing<char, int>(value => value));
    var config = new MapperConfiguration(
      cfg =>
      {
        cfg.CreateMap<BenchSource, BenchDestination>();
        cfg.CreateMap<BenchSource.Int1, BenchDestination.Int1>();
        cfg.CreateMap<BenchSource.Int2, BenchDestination.Int2>();
        cfg.CreateMap<B2, A2>();
      });
    _autoMapper = config.CreateMapper();

    _benchSource = fixture.Create<BenchSource>();
    _simpleSource = fixture.Create<B2>();
    _simple100List = fixture.CreateMany<B2>(100).ToList();
    _simple1000List = fixture.CreateMany<B2>(1000).ToList();
    _benchSources1000List = fixture.CreateMany<BenchSource>(1000).ToList();
  }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public BenchDestination EmitMapper_BenchSource()
  // {
  //     return _benchSourceEmitMapper.Map(_benchSource);
  // }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public BenchDestination AutoMapper_BenchSource()
  // {
  //     return _autoMapper.Map<BenchSource, BenchDestination>(_benchSource);
  // }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public A2 EmitMapper_Simple()
  // {
  //     return _simpleEmitMapper.Map(_simpleSource);
  // }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public A2 AutoMapper_Simple()
  // {
  //     return _autoMapper.Map<B2, A2>(_simpleSource);
  // }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public List<A2> EmitMapper_SimpleList100()
  // {
  //     return _simpleEmitMapper.MapEnum(_simple100List).ToList();
  // }
  //
  // [Benchmark(OperationsPerInvoke = IterationCount)]
  // public List<A2> AutoMapper_SimpleList100()
  // {
  //     return _autoMapper.Map<List<B2>, List<A2>>(_simple100List);
  // }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<A2> EmitMapper_SimpleList1000()
  {
    return _simpleEmitMapper.MapEnum(_simple1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<A2> AutoMapper_SimpleList1000()
  {
    return _autoMapper.Map<List<B2>, List<A2>>(_simple1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchDestination> EmitMapper_BenchSourceList1000()
  {
    return _benchSourceEmitMapper.MapEnum(_benchSources1000List);
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchDestination> AutoMapper_BenchSourceList1000()
  {
    return _autoMapper.Map<List<BenchSource>, List<BenchDestination>>(_benchSources1000List);
  }
}

/*
* Summary *

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