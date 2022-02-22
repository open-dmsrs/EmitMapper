namespace EmitMapper.Tests;

using EmitMapper.MappingConfiguration;

using Shouldly;

using Xunit;

////[TestFixture]
public class TypeConversion
{
  [Fact]
  public void Test1()
  {
    var a = new A1();
    var b = new B1();
    var mapper = Context.ObjMan.GetMapper<B1, A1>();

    // DynamicAssemblyManager.SaveAssembly();
    mapper.Map(b, a);
    a.Fld1.ShouldBe(15);
    a.Fld2.ShouldBe("11");
  }

  [Fact]
  public void Test2()
  {
    var a = new A2();
    var b = new B2();
    Context.ObjMan.GetMapper<B2, A2>().Map(b, a);

    // DynamicAssemblyManager.SaveAssembly();
    a.Fld3[0].ShouldBe("99");
  }

  [Fact]
  public void Test3_ShallowCopy()
  {
    var a = new A3();
    var b = new B3();
    b.A1.Fld1 = 15;
    Context.ObjMan.GetMapper<B3, A3>(new DefaultMapConfig().DeepMap()).Map(b, a);
    a.A1.Fld1.ShouldBe(15);
    b.A1.Fld1 = 666;
    a.A1.Fld1.ShouldBe(15);

    Context.ObjMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap()).Map(b, a);
    b.A1.Fld1 = 777;
    a.A1.Fld1.ShouldBe(777);

    b = new B3();
    Context.ObjMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap<A1>().DeepMap<A2>()).Map(b, a);
    b.A1.Fld1 = 333;
    b.A2.Fld3 = new string[1];

    a.A1.Fld1.ShouldBe(333);
    a.A2.Fld3.ShouldBeNull();
  }

  [Fact]
  public void Test4()
  {
    var a = new A4();
    var b = new B4();
    Context.ObjMan.GetMapper<B4, A4>().Map(b, a);
    a.Str.ShouldBe("string");
  }

  public class A1
  {
    public int Fld1;

    public string Fld2;
  }

  public class A2
  {
    public string[] Fld3;
  }

  public class A3
  {
    public A1 A1 = new();

    public A2 A2 = new();
  }

  public class A4
  {
    public string Str { get; set; }
  }

  public class A5
  {
    public string Fld2;
  }

  public class B1
  {
    public decimal Fld1 = 15;

    public decimal Fld2 = 11;
  }

  public class B2
  {
    public int Fld3 = 99;
  }

  public class B3
  {
    public A1 A1 = new();

    public A2 A2 = new();
  }

  public class B4
  {
    public BInt Str { get; } = new();

    public class BInt
    {
      public override string ToString()
      {
        return "string";
      }
    }
  }

  public class B5
  {
    public decimal Fld2 = 11;
  }
}