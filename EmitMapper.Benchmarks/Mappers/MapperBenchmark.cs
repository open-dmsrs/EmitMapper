using BenchmarkDotNet.Jobs;

namespace EmitMapper.Benchmarks.Mappers;

/// <summary>
/// The mapper benchmark.
/// </summary>
[SimpleJob(RuntimeMoniker.Net70, baseline: true)]
// [RPlotExporter]
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class MapperBenchmark
{
	private IMapper autoMapper;

	private BenchNestedSource benchSource;

	private Mapper<BenchNestedSource, BenchNestedDestination> benchSourceEmitMapper;

	private List<BenchNestedSource> benchSources1000List;

	private List<SimpleTypesSource> simple1000List;

	private List<SimpleTypesSource> simple100List;

	private Mapper<SimpleTypesSource, SimpleTypesDestination> simpleEmitMapper;

	private SimpleTypesSource simpleSource;

	///// <summary>
	/////   The iteration count.
	///// </summary>
	////private const int IterationCount = 1_000;

	/// <summary>
	/// Bench_a_s the hard mapper.
	/// </summary>
	/// <returns>A BenchNestedDestination.</returns>
	[BenchmarkCategory("Bench", "1")]
	[Benchmark]
	public BenchNestedDestination Bench_a_HardMapper()
	{
		return HardCodeMapper.HardMap(benchSource);
	}

	/// <summary>
	/// Bench_b_s the emit mapper.
	/// </summary>
	/// <returns>A BenchNestedDestination.</returns>
	[BenchmarkCategory("Bench", "1")]
	[Benchmark(Baseline = true)]
	public BenchNestedDestination Bench_b_EmitMapper()
	{
		return benchSourceEmitMapper.Map(benchSource);
	}

	/// <summary>
	/// Bench_c_s the auto mapper.
	/// </summary>
	/// <returns>A BenchNestedDestination.</returns>
	[BenchmarkCategory("Bench", "1")]
	[Benchmark]
	public BenchNestedDestination Bench_c_AutoMapper()
	{
		return autoMapper.Map<BenchNestedSource, BenchNestedDestination>(benchSource);
	}

	/// <summary>
	/// Benches the nested1000_a_ hard mapper.
	/// </summary>
	/// <returns><![CDATA[List<BenchNestedDestination>]]></returns>
	[BenchmarkCategory("Bench", "1000")]
	[Benchmark]
	public List<BenchNestedDestination> BenchNested1000_a_HardMapper()
	{
		return benchSources1000List.Select(s => HardCodeMapper.HardMap(s)).ToList();
	}

	/// <summary>
	/// Benches the nested1000_b_ emit mapper.
	/// </summary>
	/// <returns><![CDATA[List<BenchNestedDestination>]]></returns>
	[BenchmarkCategory("Bench", "1000")]
	[Benchmark(Baseline = true)]
	public List<BenchNestedDestination> BenchNested1000_b_EmitMapper()
	{
		return benchSourceEmitMapper.MapEnum(benchSources1000List);
	}

	/// <summary>
	/// Benches the nested1000_c_ auto mapper.
	/// </summary>
	/// <returns><![CDATA[List<BenchNestedDestination>]]></returns>
	[BenchmarkCategory("Bench", "1000")]
	[Benchmark]
	public List<BenchNestedDestination> BenchNested1000_c_AutoMapper()
	{
		return autoMapper.Map<List<BenchNestedSource>, List<BenchNestedDestination>>(benchSources1000List);
	}

	/// <summary>
	/// Setups the.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		var fixture = new Fixture();
		benchSourceEmitMapper = Mapper.Default.GetMapper<BenchNestedSource, BenchNestedDestination>();

		simpleEmitMapper =
		  Mapper.Default.GetMapper<SimpleTypesSource, SimpleTypesDestination>(new DefaultMapConfig());

		var config = new MapperConfiguration(
		  cfg =>
		  {
			  cfg.CreateMap<BenchNestedSource, BenchNestedDestination>();
			  cfg.CreateMap<BenchNestedSource.Nested2, BenchNestedDestination.Inner2>();
			  cfg.CreateMap<BenchNestedSource.Nested1, BenchNestedDestination.Inner1>();
			  cfg.CreateMap<SimpleTypesSource, SimpleTypesDestination>();
		  });

		autoMapper = config.CreateMapper();

		benchSource = fixture.Create<BenchNestedSource>();
		simpleSource = fixture.Create<SimpleTypesSource>();
		simple100List = fixture.CreateMany<SimpleTypesSource>(100).ToList();
		simple1000List = fixture.CreateMany<SimpleTypesSource>(1000).ToList();
		benchSources1000List = fixture.CreateMany<BenchNestedSource>(1000).ToList();
		this.BenchNested1000_a_HardMapper();
		this.BenchNested1000_b_EmitMapper();
		this.BenchNested1000_c_AutoMapper();
		this.Bench_a_HardMapper();
		this.Bench_b_EmitMapper();
		this.Bench_c_AutoMapper();
		this.SimpleTypes1000_a_HardMapper();
		this.SimpleTypes1000_b_EmitMapper();
		this.SimpleTypes1000_c_AutoMapper();
		this.SimpleTypes100_a_HardMapper();
		this.SimpleTypes100_b_EmitMapper();
		this.SimpleTypes100_c_AutoMapper();
		this.SimpleTypes_a_HardMapper();
		this.SimpleTypes_b_EmitMapper();
		this.SimpleTypes_c_AutoMapper();
	}

	/// <summary>
	/// Simples the types_a_ hard mapper.
	/// </summary>
	/// <returns>A SimpleTypesDestination.</returns>
	[BenchmarkCategory("SimpleTypes", "1")]
	[Benchmark]
	public SimpleTypesDestination SimpleTypes_a_HardMapper()
	{
		return HardCodeMapper.HardMap(simpleSource);
	}

	/// <summary>
	/// Simples the types_b_ emit mapper.
	/// </summary>
	/// <returns>A SimpleTypesDestination.</returns>
	[BenchmarkCategory("SimpleTypes", "1")]
	[Benchmark(Baseline = true)]
	public SimpleTypesDestination SimpleTypes_b_EmitMapper()
	{
		return simpleEmitMapper.Map(simpleSource);
	}

	/// <summary>
	/// Simples the types_c_ auto mapper.
	/// </summary>
	/// <returns>A SimpleTypesDestination.</returns>
	[BenchmarkCategory("SimpleTypes", "1")]
	[Benchmark]
	public SimpleTypesDestination SimpleTypes_c_AutoMapper()
	{
		return autoMapper.Map<SimpleTypesSource, SimpleTypesDestination>(simpleSource);
	}

	/// <summary>
	/// Simples the types100_a_ hard mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "100")]
	[Benchmark]
	public List<SimpleTypesDestination> SimpleTypes100_a_HardMapper()
	{
		return simple100List.Select(s => HardCodeMapper.HardMap(s)).ToList();
	}

	/// <summary>
	/// Simples the types100_b_ emit mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "100")]
	[Benchmark(Baseline = true)]
	public List<SimpleTypesDestination> SimpleTypes100_b_EmitMapper()
	{
		return simpleEmitMapper.MapEnum(simple100List).ToList();
	}

	/// <summary>
	/// Simples the types100_c_ auto mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "100")]
	[Benchmark]
	public List<SimpleTypesDestination> SimpleTypes100_c_AutoMapper()
	{
		return autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(simple100List);
	}

	/// <summary>
	/// Simples the types1000_a_ hard mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "1000")]
	[Benchmark]
	public List<SimpleTypesDestination> SimpleTypes1000_a_HardMapper()
	{
		return simple1000List.Select(s => HardCodeMapper.HardMap(s)).ToList();
	}

	/// <summary>
	/// Simples the types1000_b_ emit mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "1000")]
	[Benchmark(Baseline = true)]
	public List<SimpleTypesDestination> SimpleTypes1000_b_EmitMapper()
	{
		return simpleEmitMapper.MapEnum(simple1000List);
	}

	/// <summary>
	/// Simples the types1000_c_ auto mapper.
	/// </summary>
	/// <returns><![CDATA[List<SimpleTypesDestination>]]></returns>
	[BenchmarkCategory("SimpleTypes", "1000")]
	[Benchmark]
	public List<SimpleTypesDestination> SimpleTypes1000_c_AutoMapper()
	{
		return autoMapper.Map<List<SimpleTypesSource>, List<SimpleTypesDestination>>(simple1000List);
	}

	/// <summary>
	/// Usages
	/// </summary>
	public void Usage()
	{
		var simple = Mapper.Default.GetMapper<BenchNestedSource, BenchNestedDestination>();
		_ = simple.Map(benchSource);
		_ = simple.MapEnum(benchSources1000List); // for list object
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

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1806 (21H2)
Intel Core i7-3740QM CPU 2.70GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT

|                       Method |             Mean |          Error |         StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |   Allocated |
|----------------------------- |-----------------:|---------------:|---------------:|------:|--------:|---------:|---------:|------------:|
|           Bench_a_HardMapper |        818.52 ns |       4.338 ns |       4.057 ns |  1.07 |    0.01 |   0.9613 |        - |     3,016 B |
|           Bench_b_EmitMapper |        764.31 ns |       3.967 ns |       3.711 ns |  1.00 |    0.00 |   0.9613 |        - |     3,016 B |
|           Bench_c_AutoMapper |     12,889.21 ns |      51.436 ns |      45.597 ns | 16.87 |    0.09 |   0.9613 |        - |     3,016 B |
|                              |                  |                |                |       |         |          |          |             |
| BenchNested1000_a_HardMapper |  3,899,484.54 ns |  11,830.482 ns |  10,487.414 ns |  1.00 |    0.00 | 480.4688 | 238.2813 | 3,024,130 B |
| BenchNested1000_b_EmitMapper |  3,904,219.01 ns |  11,406.954 ns |   9,525.323 ns |  1.00 |    0.00 | 480.4688 | 238.2813 | 3,024,058 B |
| BenchNested1000_c_AutoMapper | 18,211,243.08 ns | 158,659.781 ns | 140,647.764 ns |  4.66 |    0.04 | 468.7500 | 218.7500 | 3,032,615 B |
|                              |                  |                |                |       |         |          |          |             |
|     SimpleTypes_a_HardMapper |         32.72 ns |       0.306 ns |       0.286 ns |  0.62 |    0.01 |   0.0382 |        - |       120 B |
|     SimpleTypes_b_EmitMapper |         52.90 ns |       0.223 ns |       0.209 ns |  1.00 |    0.00 |   0.0382 |        - |       120 B |
|     SimpleTypes_c_AutoMapper |        190.70 ns |       3.787 ns |       3.542 ns |  3.60 |    0.07 |   0.0381 |        - |       120 B |
|                              |                  |                |                |       |         |          |          |             |
|  SimpleTypes100_a_HardMapper |      4,533.49 ns |      89.790 ns |     206.307 ns |  0.66 |    0.02 |   4.1199 |        - |    12,928 B |
|  SimpleTypes100_b_EmitMapper |      7,091.54 ns |     135.759 ns |     139.414 ns |  1.00 |    0.00 |   4.3640 |        - |    13,712 B |
|  SimpleTypes100_c_AutoMapper |      6,662.15 ns |     129.392 ns |     132.876 ns |  0.94 |    0.03 |   4.5242 |        - |    14,192 B |
|                              |                  |                |                |       |         |          |          |             |
| SimpleTypes1000_a_HardMapper |     51,368.89 ns |   1,014.286 ns |     899.138 ns |  0.66 |    0.02 |  31.1890 |  10.1318 |   128,128 B |
| SimpleTypes1000_b_EmitMapper |     78,323.05 ns |   1,522.685 ns |   1,563.685 ns |  1.00 |    0.00 |  30.7617 |  10.0098 |   128,056 B |
| SimpleTypes1000_c_AutoMapper |     73,083.38 ns |   1,290.529 ns |   1,144.020 ns |  0.94 |    0.02 |  37.1094 |   6.9580 |   136,600 B |
 *
 * ******/