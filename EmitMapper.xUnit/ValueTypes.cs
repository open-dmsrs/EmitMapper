using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class ValueTypes
{
    [Fact]
    public void Test_ClassToStruct()
    {
        var a = new A1();
        var b = new B1();
        a = Context.ObjMan.GetMapper<B1, A1>().Map(b, a);
        Assert.Equal(10, a.Fld1);
    }

    [Fact]
    public void Test_NestedStructs()
    {
        var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B6From, A6To>();
        //DynamicAssemblyManager.SaveAssembly();
        var b = new B6From();

        var bs2 = new B6From.S2Struct();
        bs2.S.I = 15;
        b.S2 = bs2;
        b.S.S.I = 13;
        b.S3.S.I = 10;
        b.S4 = new();
        b.S4.S.I = 11;
        b.S5 = new() { C1 = new() };
        b.S5.C1.S.I = 1;
        b.S5.C2 = new();
        b.S5.C2.S.I = 2;
        b.S5.C3 = new();
        b.S5.C3.S.I = 3;

        var a = mapper.Map(b);
        Assert.Equal(13, a.S.S.I);
        Assert.Equal(15, a.S2.S.I);
        Assert.Equal(10, a.S3.S.I);
        Assert.Equal(11, a.S4.S.I);
        Assert.Equal(1, a.S5.C1.S.I);
        Assert.Equal(2, a.S5.C2.S.I);
        Assert.Equal(3, a.S5.C3.S.I);
    }

    [Fact]
    public void Test_StructFields()
    {
        var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B5, A5>();
        //DynamicAssemblyManager.SaveAssembly();
        var b = new B5();
        b.A.Fld1 = 10;
        var a = mapper.Map(b);
        Assert.Equal(10, a.A.Fld1);
    }

    [Fact]
    public void Test_StructProperties()
    {
        var a = new A4();
        var b = new B4();
        var mapper = Context.ObjMan.GetMapper<B4, A4>();
        //DynamicAssemblyManager.SaveAssembly();

        a = mapper.Map(b, a);
        Assert.Equal(b.Fld1.Fld1.ToString(), a.Fld1.Fld1);
        Assert.Equal(b.Fld2.Fld1.ToString(), a.Fld2.Fld1);
        Assert.Equal(b.Fld3.Fld1.ToString(), a.Fld3.Fld1);
    }

    [Fact]
    public void Test_StructToClass()
    {
        var a = new A3();
        var b = new B3 { Fld1 = 87 };
        a = Context.ObjMan.GetMapper<B3, A3>().Map(b, a);
        Assert.Equal(87, a.Fld1);
    }

    [Fact]
    public void Test_StructToStruct()
    {
        var a = new A2();
        var b = new B2 { Fld1 = 99 };
        a = Context.ObjMan.GetMapper<B2, A2>().Map(b, a);
        Assert.Equal(99, a.Fld1);
    }

    public class A6To
    {
        public C2 S4;

        public C3Class S5;
        public S2Struct S2;

        public S2Struct S { get; set; }

        public C1 S3 { get; set; }

        public struct S1
        {
            public int I { get; set; }
        }

        public struct S2Struct
        {
            public S1 S { get; set; }
        }

        public class C1
        {
            public S1 S { get; set; }
        }

        public class C2
        {
            public S1 S;
        }

        public class C3Class
        {
            public C2 C1;

            public C2 C2;

            public C2 C3;
        }
    }

    public class B6From
    {
        public C3Class S5;
        public S2Struct S = new();

        public S2Struct S3;

        public S2Struct S2 { get; set; }

        public C1 S4 { get; set; }

        public struct S1
        {
            public int I { get; set; }
        }

        public struct S2Struct
        {
            public S1 S;
        }

        public class C1
        {
            public S1 S;
        }

        public class C3Class
        {
            public C1 C1;

            public C1 C2;

            public C1 C3;
        }
    }

    public struct A1
    {
        public int Fld1;
    }

    public class B1
    {
        public int Fld1 = 10;
    }

    public struct A2
    {
        public int Fld1;
    }

    public class B2
    {
        public int Fld1;
    }

    public class A3
    {
        public int Fld1;
    }

    public struct B3
    {
        public int Fld1;
    }

    public struct A4
    {
        public Int Fld2;

        public Int Fld1 { get; set; }

        public Int Fld3 { get; set; }

        public struct Int
        {
            public string Fld1;
        }
    }

    public class B4
    {
        public Int Fld3;

        public B4()
        {
            Fld1 = new() { Fld1 = 12.444M };
            Fld2 = new() { Fld1 = 1111 };
            Fld3.Fld1 = 444;
        }

        public Int Fld1 { get; set; }

        public Int Fld2 { get; set; }

        public struct Int
        {
            public decimal Fld1;
        }
    }

    public class A5
    {
        public A1 A;
    }

    public class B5
    {
        public A1 A;
    }
}