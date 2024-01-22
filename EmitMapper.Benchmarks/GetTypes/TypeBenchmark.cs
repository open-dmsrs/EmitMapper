using BenchmarkDotNet.Jobs;
using EmitMapper.Utils;

namespace EmitMapper.Benchmarks.GetTypes;

/// <summary>
///   The type benchmark.
/// </summary>
[MediumRunJob(RuntimeMoniker.Net60)]

//[MemoryDiagnoser]
public class TypeBenchmark
{
	private Employee? e;

	/// <summary>
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		e = new Employee();
		OfGetType();
		OfMetadata();
		OfTypeof();
	}

	/// <summary>
	///   Of_s the get type.
	/// </summary>
	/// <returns>A Type.</returns>
	[Benchmark()]
	public Type OfGetType()
	{
		return e.GetType();
	}

	/// <summary>
	///   Of_s the metadata.
	/// </summary>
	/// <returns>A Type.</returns>
	[Benchmark(Baseline = true)]
	public Type? OfMetadata()
	{
		return Metadata<Employee>.Type;
	}

	/// <summary>
	///   Of_typeoves the <see cref="Type" />.
	/// </summary>
	/// <returns>A Type.</returns>
	[Benchmark()]
	public Type OfTypeof()
	{
		return typeof(Employee);
	}
}

/*/ * Summary *

BenchmarkDotNet = v0.13.1, OS = Windows 10.0.19044.1499(21H2)
Intel Core i7-3740QM CPU 2.70GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
  .NET SDK=6.0.101
  [Host]   : .NET 6.0.1(6.0.121.56705), X64 RyuJIT
  .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

  Job=.NET 6.0  Runtime=.NET 6.0

|      Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Allocated |
|------------ |----------:|----------:|----------:|----------:|------:|--------:|----------:|
|   Of_typeof | 0.0014 ns | 0.0000 ns | 0.0000 ns | 0.0014 ns |  1.00 |    0.00 |         - |
|             |           |           |           |           |       |         |           |
|  Of_GetType | 0.0015 ns | 0.0000 ns | 0.0000 ns | 0.0015 ns |  1.00 |    0.00 |         - |
|             |           |           |           |           |       |         |           |
| Of_Metadata | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |     ? |       ? |         - |


BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1806 (21H2)
Intel Core i7-3740QM CPU 2.70GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.301
  [Host]   : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT  [AttachedDebugger]
  .NET 6.0 : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0

|      Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------ |----------:|----------:|----------:|------:|--------:|
|  Of_GetType | 1.6957 ns | 0.0192 ns | 0.0160 ns |     ? |       ? |
| Of_Metadata | 0.0000 ns | 0.0000 ns | 0.0000 ns |     ? |       ? |
|   Of_typeof | 1.4109 ns | 0.0134 ns | 0.0126 ns |     ? |       ? |

  */