using System;
using EmitMapper.MappingConfiguration;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class NullableTypes
{
  [Fact]
  public void Nullable_to_Value()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B1, A1>(
      new DefaultMapConfig().NullSubstitution<B1.Int1, A1.Int1>(state => new A1.Int1(0))
        .NullSubstitution<int?, int>(state => 3).NullSubstitution<int?, int?>(state => 4));

    // DynamicAssemblyManager.SaveAssembly();
    var a = mapper.Map(new B1());
    a.Fld1.ShouldBe(10);
    a.I.ShouldNotBeNull();
    a.I.S.ShouldBe("A1::Int1::s");
    a.Fld2.ShouldBe(3);
    a.Fld3.ShouldBe(4);
  }

  [Fact]
  public void NullableStruct_to_Struct()
  {
    var bint = new B3.BInt { Fld1 = "b" };

    var b = new B3
    {
      Fld1 = bint,
      Fld2 = bint,
      Fld3 = bint,
      Fld4 = bint,
      Fld6 = bint,
      Fld7 = bint
    };

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();

    // DynamicAssemblyManager.SaveAssembly();
    var a = mapper.Map(b);
    a.Fld1.Value.Fld1.ShouldBe("b");
    a.Fld2.Value.Fld1.ShouldBe("b");
    a.Fld3.Value.Fld1.ShouldBe("b");
    a.Fld4.Value.Fld1.ShouldBe("b");
    a.Fld6.Value.Fld1.ShouldBe("b");
    a.Fld7.Fld1.ShouldBe("b");
    a.Fld2.Value.Fld3.ShouldBe("a");
    a.Fld5.HasValue.ShouldBeFalse();
  }

  [Fact]
  public void Struct_to_NullableStruct()
  {
    var bint = new B4.BInt { Fld1 = "b" };
    var b = new B4 { Fld1 = bint };
    var a = Context.ObjMan.GetMapper<B4, A4>().Map(b);
    a.Fld1.Fld1.ShouldBe("b");
  }

  [Fact]
  public void Test_Nullable()
  {
    var a = Context.ObjMan.GetMapper<B5, A5>().Map(new B5());
    a.Fld1.Value.ShouldBe(10);
    a.Fld2.ShouldBeNull();
    A5.En.Value2.ShouldBe(a.Fld3.Value);
    A5.En.Value3.ShouldBe(a.Fld4);
    a.Fld5.Value.ShouldBe(13);
    a.Fld6.Value.ShouldBe(11);
  }

  [Fact]
  public void Test_Object_Nullable()
  {
    var a = ObjectMapperManager.DefaultInstance
      .GetMapper<B6, A6>(new DefaultMapConfig().DeepMap().ConvertUsing<object, object>(v => null)).Map(new B6());

    a.ShouldBeNull();
  }

  [Fact]
  public void Test_Object_Nullable7()
  {
    var a = ObjectMapperManager.DefaultInstance
      .GetMapper<B7, A7>(new DefaultMapConfig().DeepMap().ConvertUsing<object, int>(v => 100)).Map(new B7());

    a.I.ShouldBe(100);
  }

  [Fact]
  public void Value_to_Nullable()
  {
    var a = Context.ObjMan.GetMapper<B2, A2>().Map(new B2());
    a.Fld1.ShouldBe(10);
  }

  public class A1
  {
    public int Fld1;
    public int Fld2;
    public int? Fld3;
    public Int1 I;

    public class Int1
    {
      public string S;

      public Int1()
      {
      }

      public Int1(int i)
      {
        S = "A1::Int1::s";
      }
    }
  }

  public class A2
  {
    public int? Fld1;
  }

  public class A3
  {
    public AInt? Fld1;
    public AInt? Fld4;
    public AInt? Fld5;
    public AInt? Fld6;
    public AInt Fld7;

    public A3()
    {
      var a = new AInt { Fld3 = "a" };
      Fld2 = a;
      Fld5 = a;
    }

    public AInt? Fld2 { get; set; }
    public AInt? Fld3 { get; set; }

    public struct AInt
    {
      public string Fld1;
      public string Fld2;
      public string Fld3;
    }
  }

  public class A4
  {
    public AInt Fld1;

    public struct AInt
    {
      public string Fld1;
    }
  }

  public class A5
  {
    public int? Fld1 = 0;
    public int? Fld2 = 10;
    public En? Fld3 = En.Value1;
    public En Fld4 = En.Value1;
    public int? Fld5 = 0;
    public int? Fld6 = null;

    public enum En
    {
      Value1,
      Value2,
      Value3
    }
  }

  public class A6
  {
    public DateTime? Dt { get; set; }
    public int? I { get; set; }
  }

  public class A7
  {
    public int? I { get; set; }
  }

  public class B1
  {
    public int? Fld1 = 10;
    public int? Fld2;
    public int? Fld3;
    public Int1 I;

    public class Int1
    {
      public string S = "B1::Int1::s";
    }
  }

  public class B2
  {
    public int Fld1 = 10;
  }

  public class B3
  {
    public BInt? Fld1;
    public BInt? Fld3;
    public BInt? Fld5;
    public BInt Fld6;
    public BInt? Fld7;
    public BInt? Fld2 { get; set; }
    public BInt? Fld4 { get; set; }

    public struct BInt
    {
      public string Fld1;
      public string Fld2;
    }
  }

  public class B4
  {
    public BInt? Fld1 = new BInt();

    public struct BInt
    {
      public string Fld1;
    }
  }

  public class B5
  {
    public int Fld1 = 10;
    public string Fld2 = null;
    public En Fld3 = En.Value2;
    public En? Fld4 = En.Value3;
    public int? Fld5 = 13;
    public string Fld6 = "11";

    public enum En
    {
      Value1,
      Value2,
      Value3
    }
  }

  public class B6
  {
  }

  public class B7
  {
    public decimal I = 100;
  }
}