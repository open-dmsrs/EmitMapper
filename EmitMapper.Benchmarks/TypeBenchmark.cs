namespace EmitMapper.Benchmarks;

using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using EmitMapper.Utils;

public class Employee
{
}

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[MemoryDiagnoser]
public class TypeBenchmark
{
  private Employee e;

  [GlobalSetup]
  public void Setup()
  {
    e = new Employee();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public Type Of_GetType()
  {
    return e.GetType();
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public Type Of_Metadata()
  {
    return Metadata<Employee>.Type;
  }

  [Benchmark(OperationsPerInvoke = IterationCount)]
  public Type Of_typeof()
  {
    return typeof(Employee);
  }

  private const int IterationCount = 1_000;
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

  */