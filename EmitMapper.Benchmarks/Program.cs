using BenchmarkDotNet.Running;

namespace EmitMapper.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MapperBenchmark>(
 /*
                                  ManualConfig.Create(DefaultConfig.Instance)
                                      .WithOptions(ConfigOptions.DisableOptimizationsValidator)*/
        );
    }
}