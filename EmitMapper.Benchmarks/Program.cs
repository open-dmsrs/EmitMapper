using EmitMapper.Benchmarks.Mappers;

namespace EmitMapper.Benchmarks;

  /// <summary>
  ///   The program.
  /// </summary>
  public static class Program
  {
    /// <summary>
    /// </summary>
    /// <param name="args">The args.</param>
    public static void Main(string[] args)
    {
      /*******
       *
       * if you wanna run a benchmark in VS IDE, you need to make sure that Running it ** Without Debugging **
       * 在VS IDE中运行此测试，需要确保使用非调试模式，即 Start Without Debugging
       */

      BenchmarkRunner.Run<MapperBenchmark>(

      /****
                                                         ManualConfig.Create(DefaultConfig.Instance)
                                                             .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                        /****/
      );

      //BenchmarkRunner.Run<TypeBenchmark>(

      //  /****/
      //  ManualConfig.Create(DefaultConfig.Instance)
      //  .WithOptions(ConfigOptions.DisableOptimizationsValidator)
      //  /****/
      //);
    }
  }