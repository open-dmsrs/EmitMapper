using EmitMapper.MappingConfiguration;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

/// <summary>
/// The type conversion.
/// </summary>

public class TypeConversion
{
  /// <summary>
  /// 
  /// </summary>
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

  /// <summary>
  /// 
  /// </summary>
  [Fact]
  public void Test2()
  {
    var a = new A2();
    var b = new B2();
    Context.ObjMan.GetMapper<B2, A2>().Map(b, a);

    // DynamicAssemblyManager.SaveAssembly();
    a.Fld3[0].ShouldBe("99");
  }

  /// <summary>
  /// Test3_s the shallow copy.
  /// </summary>
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

  /// <summary>
  /// 
  /// </summary>
  [Fact]
  public void Test4()
  {
    var a = new A4();
    var b = new B4();
    Context.ObjMan.GetMapper<B4, A4>().Map(b, a);
    a.Str.ShouldBe("string");
  }

  /// <summary>
  /// The a1.
  /// </summary>
  public class A1
  {
    public int Fld1;
    public string Fld2;
  }

  /// <summary>
  /// The a2.
  /// </summary>
  public class A2
  {
    public string[] Fld3;
  }

  /// <summary>
  /// The a3.
  /// </summary>
  public class A3
  {
    public A1 A1 = new();
    public A2 A2 = new();
  }

  /// <summary>
  /// The a4.
  /// </summary>
  public class A4
  {
    /// <summary>
    /// Gets or Sets the str.
    /// </summary>
    public string Str { get; set; }
  }

  /// <summary>
  /// The a5.
  /// </summary>
  public class A5
  {
    public string Fld2;
  }

  /// <summary>
  /// The b1.
  /// </summary>
  public class B1
  {
    public decimal Fld1 = 15;
    public decimal Fld2 = 11;
  }

  /// <summary>
  /// The b2.
  /// </summary>
  public class B2
  {
    public int Fld3 = 99;
  }

  /// <summary>
  /// The b3.
  /// </summary>
  public class B3
  {
    public A1 A1 = new();
    public A2 A2 = new();
  }

  /// <summary>
  /// The b4.
  /// </summary>
  public class B4
  {
    /// <summary>
    /// Gets the str.
    /// </summary>
    public BInt Str { get; } = new();

    /// <summary>
    /// The b int.
    /// </summary>
    public class BInt
    {
      /// <summary>
      /// Tos the string.
      /// </summary>
      /// <returns>A string.</returns>
      public override string ToString()
      {
        return "string";
      }
    }
  }

  /// <summary>
  /// The b5.
  /// </summary>
  public class B5
  {
    public decimal Fld2 = 11;
  }
}