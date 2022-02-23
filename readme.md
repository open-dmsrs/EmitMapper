# EmitMapper
## Project Description
[![publish to nuget](https://github.com/niubilitynetcore/EmitMapper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/niubilitynetcore/EmitMapper/actions/workflows/dotnet.yml)
 [![NuGet](http://img.shields.io/nuget/v/Niubility.EmitMapper.svg)](https://www.nuget.org/packages/Niubility.EmitMapper/)

## What is EmitMapper

EmitMapper is tiny library for resolving 

Powerful customizable tool for mapping entities to each other. Entities can be plain objects, DataReaders, SQL commands and anything you need. The tool uses run-time code generation via the Emit library. It is useful for dealing with DTO objects, data access layers an so on.

## How do I get started?


```c#
	var simple = Mapper.Default.GetMapper<BenchNestedSource, BenchNestedDestination>();
    BenchNestedDestination dest = simple.Map(_benchSource); //for single object;
    List<BenchNestedDestination> dests = simple.MapEnum(_benchSources1000List);// for list object
```

## Supported platforms:

* NETStandard 2.1
* .Net Framework 4.8
* net 6.0
* netcore app 3.1

### Where can I get it?
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [EmitMapper](https://www.nuget.org/packages/Niubility.EmitMapper/) from the package manager console:
```dos
Install-Package Niubility.EmitMapper
```

## About Emit Mapper

* Overview
* Benefits of EmitMapper
* Getting started
* Type conversion
* Customization

# Customization overview

Customization using default configurator
* Default configurator overview
* Custom converters
* Custom converters_for_generics
* Null substitution
* Ignoring members
* Custom constructors
* Shallow and_deep_mapping
* Names matching
* Post processing

Low-level customization using custom configuratorors

# Emit Mapper in practice.

* Benchmark: EmitMapper vs Handwritten code vs AutoMapper
* Objects change tracking
* Mapping DbDatareader to objects
* Mapping objects to DbCommand (UPDATE and INSERT) Last edited Jan 11, 2010 at 3:01 PM by romankovs

## Benchmark
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.2037 (1909/November2019Update/19H2)
Intel Core i5-8350U CPU 1.70GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]   : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0 : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
Job=.NET 6.0  Runtime=.NET 6.0  
```
|                       Method |           Mean |       Error |      StdDev |         Median | Ratio |  Gen 0 |  Gen 1 | Allocated |
|----------------------------- |---------------:|------------:|------------:|---------------:|------:|-------:|-------:|----------:|
|     BenchNested_a_HardMapper |      0.8563 ns |   0.0173 ns |   0.0341 ns |      0.8489 ns |  1.00 | 0.0010 |      - |       3 B |
|     BenchNested_b_EmitMapper |      0.8897 ns |   0.0431 ns |   0.1264 ns |      0.8451 ns |  1.00 | 0.0010 |      - |       3 B |
|     BenchNested_c_AutoMapper |     16.5678 ns |   0.7823 ns |   2.2945 ns |     16.7384 ns |  1.00 | 0.0009 |      - |       3 B |
| BenchNested1000_a_HardMapper |  3,988.9549 ns |  79.6474 ns |  97.8142 ns |  3,965.5109 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
| BenchNested1000_b_EmitMapper |  3,781.9851 ns |  74.5756 ns | 122.5298 ns |  3,776.3773 ns |  1.00 | 0.4766 | 0.2344 |   3,024 B |
| BenchNested1000_c_AutoMapper | 16,978.1985 ns | 333.1485 ns | 396.5896 ns | 16,966.0594 ns |  1.00 | 0.4688 | 0.2188 |   3,033 B |
|     SimpleTypes_a_HardMapper |      0.0310 ns |   0.0008 ns |   0.0023 ns |      0.0307 ns |  1.00 | 0.0000 |      - |         - |
|     SimpleTypes_b_EmitMapper |      0.0512 ns |   0.0009 ns |   0.0010 ns |      0.0513 ns |  1.00 | 0.0000 |      - |         - |
|     SimpleTypes_c_AutoMapper |      0.1718 ns |   0.0035 ns |   0.0076 ns |      0.1709 ns |  1.00 | 0.0000 |      - |         - |
|  SimpleTypes100_a_HardMapper |      4.0774 ns |   0.0804 ns |   0.1714 ns |      4.0509 ns |  1.00 | 0.0041 |      - |      13 B |
|  SimpleTypes100_b_EmitMapper |      8.4392 ns |   0.4806 ns |   1.4020 ns |      8.3280 ns |  1.00 | 0.0044 |      - |      14 B |
|  SimpleTypes100_c_AutoMapper |      8.4149 ns |   0.5269 ns |   1.5285 ns |      8.0763 ns |  1.00 | 0.0045 |      - |      14 B |
| SimpleTypes1000_a_HardMapper |     53.3382 ns |   1.0229 ns |   2.8684 ns |     52.6775 ns |  1.00 | 0.0320 | 0.0103 |     128 B |
| SimpleTypes1000_b_EmitMapper |     81.6188 ns |   1.9400 ns |   5.5036 ns |     79.9001 ns |  1.00 | 0.0308 | 0.0101 |     128 B |
| SimpleTypes1000_c_AutoMapper |     89.3916 ns |   4.3419 ns |  12.6655 ns |     86.0168 ns |  1.00 | 0.0369 | 0.0082 |     137 B |

## Changelog

* Feb 8, 2022 The newest benchmark test is shown that the performance problem has been resolved. to see the method ToInt32(decimal?) of class NullableConverter
* Add github action for publish the package to nuget.org automatically

