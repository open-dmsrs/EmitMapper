using System;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using EmitMapper.Benchmarks.TestData;

namespace EmitMapper.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}
