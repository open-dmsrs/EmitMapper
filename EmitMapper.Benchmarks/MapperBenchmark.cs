using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EmitMapper.Benchmarks.TestData;

namespace EmitMapper.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
public class MapperBenchmark
{
    private const int IterationCount = 1_000;
    private B2 _simpleSource;

    private BenchSource _benchSource;
    private IMapper _autoMapper;
    private List<B2> _simple1000List;

    private List<B2> _simple100List;
    private ObjectsMapper<B2, A2> _simpleEmitMapper;

    private ObjectsMapper<BenchSource, BenchDestination> _benchSourceEmitMapper;

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine($"Current:{DateTime.Now.Ticks.ToString()}");

        var fixture = new Fixture();
        _benchSourceEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchSource, BenchDestination>();
        _simpleEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>();
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
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public BenchDestination EmitMapper_BenchSource()
    {
        return _benchSourceEmitMapper.Map(_benchSource);
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public BenchDestination AutoMapper_BenchSource()
    {
        return _autoMapper.Map<BenchSource, BenchDestination>(_benchSource);
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public A2 EmitMapper_Simple()
    {
        return _simpleEmitMapper.Map(_simpleSource);
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public A2 AutoMapper_Simple()
    {
        return _autoMapper.Map<B2, A2>(_simpleSource);
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public List<A2> EmitMapper_SimpleList100()
    {
        return _simpleEmitMapper.MapEnum(_simple100List).ToList();
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public List<A2> AutoMapper_SimpleList100()
    {
        return _autoMapper.Map<List<B2>, List<A2>>(_simple100List);
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public List<A2> EmitMapper_SimpleList1000()
    {
        return _simpleEmitMapper.MapEnum(_simple1000List).ToList();
    }

    [Benchmark(OperationsPerInvoke = IterationCount)]
    public List<A2> AutoMapper_SimpleList1000()
    {
        return _autoMapper.Map<List<B2>, List<A2>>(_simple1000List);
    }
}