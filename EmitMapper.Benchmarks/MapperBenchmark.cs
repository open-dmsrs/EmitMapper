using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using EmitMapper.Benchmarks.TestObject;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Benchmarks;

// [SimpleJob(RuntimeMoniker.Net60, baseline: true)]

// [RPlotExporter]
[MemoryDiagnoser]
[MediumRunJob]
[SkewnessColumn]
[KurtosisColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class MapperBenchmark
{
  private IMapper _autoMapper;

  private BenchNestedSource _benchSource;

  private ObjectsMapper<BenchNestedSource, BenchNestedDestination> _benchSourceEmitMapper;

  private List<BenchNestedSource> _benchSources1000List;

  private List<SimpleTypesSource> _simple1000List;

  private List<SimpleTypesSource> _simple100List;

  private ObjectsMapper<SimpleTypesSource, SimpleTypesDestination> _simpleEmitMapper;

  private SimpleTypesSource _simpleSource;

  private const int IterationCount = 1_000;

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

  public void Usage()
  {
    var simple = ObjectMapperManager.DefaultInstance.GetMapper<BenchNestedSource, BenchNestedDestination>();
    var dest = simple.Map(_benchSource); // for single object;
    var dests = simple.MapEnum(_benchSources1000List); // for list object
  }

  [BenchmarkCategory("Bench", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination Bench_a_HardMapper()
  {
    return HardCodeMapper.HardMap(_benchSource);
  }

  [BenchmarkCategory("Bench", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount, Baseline = true)]
  public BenchNestedDestination Bench_b_EmitMapper()
  {
    return _benchSourceEmitMapper.Map(_benchSource);
  }

  [BenchmarkCategory("Bench", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public BenchNestedDestination Bench_c_AutoMapper()
  {
    return _autoMapper.Map<BenchNestedSource, BenchNestedDestination>(_benchSource);
  }

  [BenchmarkCategory("Bench", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> BenchNested1000_a_HardMapper()
  {
    return _benchSources1000List.Select(s => HardCodeMapper.HardMap(s)).ToList();
  }

  [BenchmarkCategory("Bench", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount, Baseline = true)]
  public List<BenchNestedDestination> BenchNested1000_b_EmitMapper()
  {
    return _benchSourceEmitMapper.MapEnum(_benchSources1000List);
  }

  [BenchmarkCategory("Bench", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<BenchNestedDestination> BenchNested1000_c_AutoMapper()
  {
    return _autoMapper.Map<List<BenchNestedSource>, List<BenchNestedDestination>>(_benchSources1000List);
  }

  [BenchmarkCategory("SimpleTypes", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination SimpleTypes_a_HardMapper()
  {
    return HardCodeMapper.HardMap(_simpleSource);
  }

  [BenchmarkCategory("SimpleTypes", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount, Baseline = true)]
  public SimpleTypesDestination SimpleTypes_b_EmitMapper()
  {
    return _simpleEmitMapper.Map(_simpleSource);
  }

  [BenchmarkCategory("SimpleTypes", "1")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public SimpleTypesDestination SimpleTypes_c_AutoMapper()
  {
    return _autoMapper.Map<SimpleTypesSource, SimpleTypesDestination>(_simpleSource);
  }

  [BenchmarkCategory("SimpleTypes", "100")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes100_a_HardMapper()
  {
    return _simple100List.Select(s => HardCodeMapper.HardMap(s)).ToList();
  }

  [BenchmarkCategory("SimpleTypes", "100")]
  [Benchmark(OperationsPerInvoke = IterationCount, Baseline = true)]
  public List<SimpleTypesDestination> SimpleTypes100_b_EmitMapper()
  {
    return _simpleEmitMapper.MapEnum(_simple100List).ToList();
  }

  [BenchmarkCategory("SimpleTypes", "100")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes100_c_AutoMapper()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple100List);
  }

  [BenchmarkCategory("SimpleTypes", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes1000_a_HardMapper()
  {
    return _simple1000List.Select(s => HardCodeMapper.HardMap(s)).ToList();
  }

  [BenchmarkCategory("SimpleTypes", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount, Baseline = true)]
  public List<SimpleTypesDestination> SimpleTypes1000_b_EmitMapper()
  {
    return _simpleEmitMapper.MapEnum(_simple1000List);
  }

  [BenchmarkCategory("SimpleTypes", "1000")]
  [Benchmark(OperationsPerInvoke = IterationCount)]
  public List<SimpleTypesDestination> SimpleTypes1000_c_AutoMapper()
  {
    return _autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(_simple1000List);
  }
}

/*******
// * Summary *
   
   BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.2094 (1909/November2019Update/19H2)
   Intel Core i5-8350U CPU 1.70GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
   .NET SDK=6.0.101
   [Host]    : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
   MediumRun : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
   
   Job=MediumRun  IterationCount=15  LaunchCount=2
   WarmupCount=10
   
   |                       Method |           Mean |         Error |        StdDev | Skewness | Kurtosis | Ratio | RatioSD |  Gen 0 |  Gen 1 | Allocated |
   |----------------------------- |---------------:|--------------:|--------------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|
   |           Bench_a_HardMapper |      1.3408 ns |     0.0925 ns |     0.1296 ns |   0.8668 |    3.122 |  1.27 |    0.14 | 0.0010 |      - |       3 B |
   |           Bench_b_EmitMapper |      1.0550 ns |     0.0365 ns |     0.0535 ns |   0.5665 |    2.435 |  1.00 |    0.00 | 0.0010 |      - |       3 B |
   |           Bench_c_AutoMapper |     13.6110 ns |     0.4939 ns |     0.7084 ns |   0.7289 |    2.824 | 12.90 |    0.87 | 0.0010 |      - |       3 B |
   |                              |                |               |               |          |          |       |         |        |        |           |
   | BenchNested1000_a_HardMapper |  4,229.8534 ns |   177.1657 ns |   254.0857 ns |   1.4542 |    4.396 |  1.04 |    0.07 | 0.4766 | 0.2344 |   3,024 B |
   | BenchNested1000_b_EmitMapper |  4,087.9781 ns |   150.8346 ns |   221.0914 ns |   0.8734 |    2.682 |  1.00 |    0.00 | 0.4766 | 0.2344 |   3,024 B |
   | BenchNested1000_c_AutoMapper | 18,247.5845 ns | 2,009.6275 ns | 2,882.1477 ns |   1.4203 |    4.165 |  4.49 |    0.73 | 0.4688 | 0.2188 |   3,033 B |
   |                              |                |               |               |          |          |       |         |        |        |           |
   |     SimpleTypes_a_HardMapper |      0.0548 ns |     0.0111 ns |     0.0156 ns |   1.5642 |    4.095 |  0.79 |    0.22 | 0.0000 |      - |         - |
   |     SimpleTypes_b_EmitMapper |      0.0697 ns |     0.0022 ns |     0.0033 ns |   0.3263 |    1.876 |  1.00 |    0.00 | 0.0000 |      - |         - |
   |     SimpleTypes_c_AutoMapper |      0.2089 ns |     0.0172 ns |     0.0253 ns |   0.8055 |    3.191 |  3.00 |    0.42 | 0.0000 |      - |         - |
   |                              |                |               |               |          |          |       |         |        |        |           |
   |  SimpleTypes100_a_HardMapper |      5.6638 ns |     0.2080 ns |     0.2848 ns |   1.5036 |    6.342 |  0.67 |    0.04 | 0.0041 |      - |      13 B |
   |  SimpleTypes100_b_EmitMapper |      8.4165 ns |     0.2589 ns |     0.3714 ns |   1.1151 |    5.142 |  1.00 |    0.00 | 0.0044 |      - |      14 B |
   |  SimpleTypes100_c_AutoMapper |      8.4421 ns |     0.1799 ns |     0.2638 ns |   0.6400 |    2.709 |  1.01 |    0.06 | 0.0045 |      - |      14 B |
   |                              |                |               |               |          |          |       |         |        |        |           |
   | SimpleTypes1000_a_HardMapper |     67.4600 ns |     2.5764 ns |     3.6117 ns |   1.0069 |    4.423 |  0.70 |    0.06 | 0.0313 | 0.0103 |     128 B |
   | SimpleTypes1000_b_EmitMapper |     96.0613 ns |     4.2110 ns |     6.1724 ns |   0.9862 |    3.125 |  1.00 |    0.00 | 0.0309 | 0.0101 |     128 B |
   | SimpleTypes1000_c_AutoMapper |     99.7673 ns |     7.0173 ns |    10.5032 ns |   1.7057 |    5.107 |  1.04 |    0.12 | 0.0370 | 0.0083 |     137 B |
   
   // * Warnings *
   MultimodalDistribution
   MapperBenchmark.SimpleTypes_c_AutoMapper: MediumRun -> It seems that the distribution is bimodal (mValue = 3.83)
   
   // * Hints *
   Outliers
   MapperBenchmark.Bench_a_HardMapper: MediumRun           -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 1, Size(Result) = 27), OutlierMode = RemoveUpper
   MapperBenchmark.Bench_b_EmitMapper: MediumRun           -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 2, Size(Result) = 29), OutlierMode = RemoveUpper
   MapperBenchmark.Bench_c_AutoMapper: MediumRun           -> 2 outliers were removed (16.25 ns, 18.02 ns)
   MapperBenchmark.BenchNested1000_a_HardMapper: MediumRun -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 4, Size(Result) = 28), OutlierMode = RemoveUpper
   MapperBenchmark.BenchNested1000_b_EmitMapper: MediumRun -> 1 outlier  was  removed (5.00 us)
   MapperBenchmark.BenchNested1000_c_AutoMapper: MediumRun -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 1, Size(Result) = 28), OutlierMode = RemoveUpper
   MapperBenchmark.SimpleTypes_b_EmitMapper: MediumRun     -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 0, Size(Result) = 29), OutlierMode = RemoveUpper
   MapperBenchmark.SimpleTypes_c_AutoMapper: MediumRun     -> 1 outlier  was  removed (0.32 ns)
   MapperBenchmark.SimpleTypes100_a_HardMapper: MediumRun  -> 4 outliers were removed (6.67 ns..8.48 ns)
   MapperBenchmark.SimpleTypes100_b_EmitMapper: MediumRun  -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 3, Size(Result) = 28), OutlierMode = RemoveUpper
   MapperBenchmark.SimpleTypes100_c_AutoMapper: MediumRun  -> 1 outlier  was  removed (9.30 ns)
   MapperBenchmark.SimpleTypes1000_a_HardMapper: MediumRun -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 2, Size(Result) = 27), OutlierMode = RemoveUpper
   MapperBenchmark.SimpleTypes1000_b_EmitMapper: MediumRun -> 1 outlier  was  removed (111.62 ns)
   MapperBenchmark.SimpleTypes1000_c_AutoMapper: MediumRun -> Something went wrong with outliers: Size(WorkloadActual) = 30, Size(WorkloadActual/Outliers) = 3, Size(Result) = 30), OutlierMode = RemoveUpper
   
   // * Legends *
   Mean      : Arithmetic mean of all measurements
   Error     : Half of 99.9% confidence interval
   StdDev    : Standard deviation of all measurements
   Skewness  : Measure of the asymmetry (third standardized moment)
   Kurtosis  : Measure of the tailedness ( fourth standardized moment)
   Ratio     : Mean of the ratio distribution ([Current]/[Baseline])
   RatioSD   : Standard deviation of the ratio distribution ([Current]/[Baseline])
   Gen 0     : GC Generation 0 collects per 1000 operations
   Gen 1     : GC Generation 1 collects per 1000 operations
   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
   1 ns      : 1 Nanosecond (0.000000001 sec)


 * 
 * ******/