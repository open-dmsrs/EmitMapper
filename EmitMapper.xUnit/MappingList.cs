using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;

public class MappingList
{
  private readonly ITestOutputHelper _testOutputHelper;

  public MappingList(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Theory]
  [AutoData]
  public void TestCopyList(List<FromClass> listFrom)
  {
    _testOutputHelper.WriteLine(listFrom.Count.ToString());

    IEnumerable<IMappingOperation> Get()
    {
      yield return new ReadWriteSimple
      {
        Source = new MemberDescriptor(
          new[]
          {
            typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
            typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[
              0]
          }
        ),
        Destination = new MemberDescriptor(new[] { typeof(ToClass).GetMember(nameof(ToClass.Message))[0] })
      };


      yield return new ReadWriteSimple { Source = new MemberDescriptor(new[] { typeof(FromClass).GetMember(nameof(FromClass.Inner))[0], typeof(FromClass.InnerClass).GetMember(nameof(FromClass.InnerClass.GetMessage2))[0] }), Destination = new MemberDescriptor(new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] }) };
    }

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<List<FromClass>, List<ToClass>>(
      new CustomMapConfig
      {
        GetMappingOperationFunc = (from, to) => Get()
      });

    var tolist = mapper.Map(listFrom);
    using var f = listFrom.GetEnumerator();
    using var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine(t.Current.Message);
      Assert.Equal(f.Current.Inner.Message, t.Current.Message);
      Assert.Equal(f.Current.Inner.GetMessage2(), t.Current.Message2);
    }
  }

  //[Theory]
  // [AutoData]
  public void TestCopyList1(List<FromClass> list)
  {
    ArrayList listFrom = new(list.ToArray());


    _testOutputHelper.WriteLine(listFrom.Count.ToString());

    var rw1 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message))[0] })
    };


    var rw2 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(
            nameof(FromClass.InnerClass.GetMessage2))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] })
    };

    IEnumerable<IMappingOperation> Get()
    {
      yield return rw1;
      yield return rw2;
    }
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ArrayList, ArrayList>(
      new CustomMapConfig
      {
        GetMappingOperationFunc = (from, to) => Get()
      });

    var tolist = mapper.Map(listFrom);
    var f = listFrom.GetEnumerator();
    var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine((t.Current as ToClass)?.Message);
      Assert.Equal(((FromClass)f.Current)?.Inner.Message, (t.Current as ToClass)?.Message);
      Assert.Equal(((FromClass)f.Current)?.Inner.GetMessage2(), (t.Current as ToClass)?.Message2);
    }
  }


  [Theory]
  [AutoData]
  public void TestCopyEnum(List<FromClass> list)
  {
    _testOutputHelper.WriteLine(list.Count.ToString());

    var rw1 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[
            0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message))[0] })
    };


    var rw2 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(
            nameof(FromClass.InnerClass.GetMessage2))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] })
    };

    IEnumerable<IMappingOperation> Get()
    {
      yield return rw1;
      yield return rw2;
    }

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<FromClass, ToClass>(
      new CustomMapConfig
      {
        GetMappingOperationFunc = (from, to) => Get()
      });

    var tolist = mapper.MapEnum(list);
    using var f = list.GetEnumerator();
    using var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine(t.Current.Message);
      Assert.Equal(f.Current.Inner.Message, t.Current.Message);
      Assert.Equal(f.Current.Inner.GetMessage2(), t.Current.Message2);
    }
  }

  [Fact]
  public void TestDefaultConfigWithEnum()
  {
    Fixture fixture = new();
    var list = fixture.CreateMany<B2Source>(3).ToList();
    //list.FirstOrDefault().N5 = 3.3232423424234M;
    _testOutputHelper.WriteLine(list.Count.ToString());

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B2Source, A2Destination>();
    mapper = ObjectMapperManager.DefaultInstance.GetMapper<B2Source, A2Destination>();
    var tolist = mapper.MapEnum(list);
    using var f = list.GetEnumerator();
    using var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine(f.Current?.N5.ToString());
      var fv = ReflectionUtils.GetPublicFieldsAndProperties(typeof(B2Source))
        .Select(
          m => new
          {
            m.Name,
            FValue = (m as FieldInfo)?.GetValue(f.Current)?.ToString()
          });

      var tv = ReflectionUtils.GetPublicFieldsAndProperties(typeof(A2Destination)).Select(
        m => new
        {
          m.Name,
          TValue = (m as FieldInfo)?.GetValue(t.Current)?.ToString()
        });

      var result = fv
        .Join(tv, a => a.Name, b => b.Name, (a, b) => new { a.Name, a.FValue, b.TValue });

      foreach (var temp in result) Assert.Equal(temp.FValue, temp.TValue);
    }
  }

  public class B2Source
  {
    public byte N4;
    public decimal? N5;
    public float N6;

    public int N1;
    public int N7;
    public long N2;
    public short N3;
    public string Str1;
    public string Str2;
    public string Str3;
    public string Str4;
    public string Str5;
    public string Str6;
    public string Str7;
    public string Str8;
    public string Str9;
  }

  public class A2Destination
  {
    public int N1;
    public int N2;
    public int N3;
    public int N4;
    public int N5;
    public int N6;
    public int N7;
    public string Str1;
    public string Str2;
    public string Str3;
    public string Str4;
    public string Str5;
    public string Str6;
    public string Str7;
    public string Str8;
    public string Str9;
  }

  public class ToClass
  {
    public string Message;

    public string Message2;
  }

  public class FromClass
  {
    public InnerClass Inner = new();

    public class InnerClass
    {
      public string Message = "hello";

      public string GetMessage2()
      {
        return "medved";
      }
    }
  }
}