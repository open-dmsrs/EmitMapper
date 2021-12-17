using System;
using System.Diagnostics;
using AutoMapper;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace Benchmarks;

public class CustomizationTest
{
    private static ObjectsMapper<B2, A2> emitMapper;
    private static IMapper autoMapper;

    private static long EmitMapper_Custom(int mappingsCount)
    {
        var s = new B2();
        var d = new A2();

        var sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i < mappingsCount; ++i) d = emitMapper.Map(s, d);
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long AutoMapper_Custom(int mappingsCount)
    {
        var s = new B2();
        var d = new A2();

        var sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i < mappingsCount; ++i) d = autoMapper.Map(s, d);
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    public static void Initialize()
    {
        emitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
            new DefaultMapConfig()
                .ConstructBy(() => new A2.Int(0))
                .NullSubstitution<decimal?, decimal>(state => 42)
                .NullSubstitution<bool?, bool>(state => true)
                .NullSubstitution<int?, int>(state => 42)
                .NullSubstitution<long?, long>(state => 42)
                .ConvertUsing<long, int>(value => (int)value + 1)
                .ConvertUsing<short, int>(value => value + 1)
                .ConvertUsing<byte, int>(value => value + 1)
                .ConvertUsing<decimal, int>(value => (int)value + 1)
                .ConvertUsing<float, int>(value => (int)value + 1)
                .ConvertUsing<char, int>(value => value + 1)
        );
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<A2, B2>();
                cfg.CreateMap<B2.Int, A2.Int>().ConstructUsing(s => new A2.Int(0));

                cfg.CreateMap<long, int>().ConstructUsing(s => (int)s + 1);
                cfg.CreateMap<short, int>().ConstructUsing(s => s + 1);
                cfg.CreateMap<byte, int>().ConstructUsing(s => s + 1);
                cfg.CreateMap<decimal, int>().ConstructUsing(s => (int)s + 1);
                cfg.CreateMap<float, int>().ConstructUsing(s => (int)s + 1);
                cfg.CreateMap<char, int>().ConstructUsing(s => s + 1);

                cfg.CreateMap<B2, A2>()
                    .ForMember(s => s.nullable1, opt => opt.NullSubstitute((decimal)42))
                    .ForMember(s => s.nullable2, opt => opt.NullSubstitute(true))
                    .ForMember(s => s.nullable3, opt => opt.NullSubstitute(42))
                    .ForMember(s => s.nullable4, opt => opt.NullSubstitute((long)42));
            });
        autoMapper = config.CreateMapper();
    }

    public static void Run()
    {
        var mappingsCount = 1000000;
        Console.WriteLine("Auto Mapper (Custom): {0} milliseconds", AutoMapper_Custom(mappingsCount));
        Console.WriteLine("Emit Mapper (Custom): {0} milliseconds", EmitMapper_Custom(mappingsCount));
    }

    public class A2
    {
        public bool nullable2;

        public decimal nullable1;

        public int n1;
        public int n2;
        public int n3;
        public int n4;
        public int n5;
        public int n6;
        public int n7;
        public int n8;
        public int nullable3;

        public Int i1;
        public Int i2;
        public Int i3;
        public Int i4;
        public Int i5;
        public long nullable4;

        public string str1;

        public struct Int
        {
            public int i;

            public Int(int i)
            {
                this.i = i;
            }
        }
    }

    public class B2
    {
        public bool? nullable2;
        public byte n4 = 4;
        public char n8 = 'a';
        public decimal n5 = 5;

        public decimal? nullable1;
        public float n6 = 6;
        public int n1 = 1;
        public int n7 = 7;

        public Int i1 = new(42);
        public Int i2 = new(42);
        public Int i3 = new(42);
        public Int i4 = new(42);
        public Int i5 = new(42);
        public int? nullable3;
        public long n2 = 2;
        public long? nullable4;
        public short n3 = 3;

        public string str1 = "str1";

        public struct Int
        {
            public int i;

            public Int(int i)
            {
                this.i = i;
            }
        }
    }
}
/*
Auto Mapper (Custom): 50142 milliseconds
Emit Mapper (Custom): 197 milliseconds
*/