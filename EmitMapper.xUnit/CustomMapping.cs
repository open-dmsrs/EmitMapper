using EmitMapper.MappingConfiguration;
using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class CustomMapping
{
  [Fact]
  public void Test_CustomConverter()
  {
    var a = Context.ObjMan.GetMapper<B2, A2>(
      new DefaultMapConfig().ConvertUsing<object, string>(v => "333")
        .ConvertUsing<object, string>(v => "hello").SetConfigName("ignore")).Map(new B2());
    Assert.Null(a.Fld1);
    Assert.Equal("hello", a.Fld2);

    a = Context.ObjMan.GetMapper<B2, A2>().Map(new B2());
    Assert.Equal("B2::fld2", a.Fld2);
  }

  [Fact]
  public void Test_CustomConverter2()
  {
    var a = Context.ObjMan.GetMapper<Bb, Aa>(
      new DefaultMapConfig().ConvertUsing<object, string>(v => "converted " + v)).Map(new Bb());
    Assert.Equal("converted B2::fld1", a.Fld1);
    Assert.Equal("converted B2::fld2", a.Fld2);
  }

  [Fact]
  public void Test_CustomConverterWithInterfaces()
  {
    var str = Context.ObjMan.GetMapper<WithName, string>(
        new DefaultMapConfig().ConvertUsing<IWithName, string>(v => v.Name).SetConfigName("withinterfaces"))
      .Map(new WithName { Name = "thisIsMyName" });

    Assert.Equal("thisIsMyName", str);
  }

  [Fact]
  public void Test_PostProcessing()
  {
    var a = Context.ObjMan.GetMapper<B3, A3>(
      new DefaultMapConfig().PostProcess<A3.Int>(
        (i, state) =>
        {
          i.Str2 = "processed";
          return i;
        }).PostProcess<A3.SInt?>(
        (i, state) => { return new A3.SInt { Str1 = i.Value.Str1, Str2 = "processed" }; }).PostProcess<A3>(
        (i, state) =>
        {
          i.Status = "processed";
          return i;
        })).Map(new B3());
    Assert.Equal("B3::Int::str1", a.Fld.Str1);
    Assert.Equal("processed", a.Fld.Str2);

    Assert.Equal("B3::SInt::str1", a.Fld2.Value.Str1);
    Assert.Equal("processed", a.Fld2.Value.Str2);

    Assert.Equal("processed", a.Status);
  }

  public class A1
  {
    public string Fld1 = string.Empty;

    public string Fld2 { get; private set; } = string.Empty;

    public void SetFld2(string value)
    {
      Fld2 = value;
    }
  }

  public class A2
  {
    public string Fld1;

    public string Fld2;
  }

  public class B2
  {
    public string Fld2 = "B2::fld2";

    public string Fld3 = "B2::fld3";
  }

  public interface IWithName
  {
    string Name { get; set; }
  }

  public class WithName : IWithName
  {
    public string Name { get; set; }
  }

  public class Aa
  {
    public string Fld1;

    public string Fld2;
  }

  public class Bb
  {
    public string Fld1 = "B2::fld1";

    public string Fld2 = "B2::fld2";
  }

  public class A3
  {
    public Int Fld;

    public SInt? Fld2;

    public string Status;

    public struct SInt
    {
      public string Str1;

      public string Str2;
    }

    public class Int
    {
      public string Str1;

      public string Str2;
    }
  }

  public class B3
  {
    public Int Fld = new();

    public SInt Fld2;

    public B3()
    {
      Fld2.Str1 = "B3::SInt::str1";
    }

    public struct SInt
    {
      public string Str1;
    }

    public class Int
    {
      public string Str1 = "B3::Int::str1";
    }
  }
}