using BenchmarkDotNet.Running;

namespace EmitMapper.Benchmarks;

public class Program
{
  public static void Main(string[] args)
  {
    /*******
     *
* Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1977 (1909/November2019Update/19H2)
Intel Core i5-8350U CPU 1.70GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
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

     * if you wanna run a benchmark in VS IDE, you need to make sure Run it ** Without Debugging **
     * 在VS IDE中运行此测试，需要确保使用非调试模式，即 Start Without Debugging
     */

    BenchmarkRunner.Run<MapperBenchmark>(

      /****
                                       ManualConfig.Create(DefaultConfig.Instance)
                                           .WithOptions(ConfigOptions.DisableOptimizationsValidator)
      /****/
    );
  }
}