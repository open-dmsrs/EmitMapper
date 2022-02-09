using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using EmitMapper.Common.TestData;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
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

    var rw1 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        typeof(FromClass).GetMember(nameof(FromClass.Inner))[0].AsEnumerable(
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[0])
      ),
      Destination = new MemberDescriptor(typeof(ToClass).GetMember(nameof(ToClass.Message))[0].AsEnumerable())
    };


    var rw2 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        typeof(FromClass).GetMember(nameof(FromClass.Inner))[0].AsEnumerable(
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.InnerClass.GetMessage2))[0])
      ),
      Destination = new MemberDescriptor(typeof(ToClass).GetMember(nameof(ToClass.Message2))[0].AsEnumerable())
    };


    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<List<FromClass>, List<ToClass>>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

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
  private void TestCopyList1(List<FromClass> list)
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
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0], typeof(FromClass.InnerClass).GetMember(
            nameof(FromClass.InnerClass.GetMessage2))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] })
    };


    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ArrayList, ArrayList>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

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
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0], typeof(FromClass.InnerClass).GetMember(
            nameof(FromClass.InnerClass.GetMessage2))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] })
    };


    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<FromClass, ToClass>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

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
  public void ConvertCharToInt32()
  {
    char m = 'a';
    int n = Convert.ToInt32(m);

    _testOutputHelper.WriteLine(n + "");
  }

  [Fact]
  public void TestManyValueType()
  {
    Fixture fixture = new();
    //fixture.Customizations.Add(
    //  new RandomDoublePrecisionFloatingPointSequenceGenerator());

    var list = fixture.CreateMany<SimpleTypesSource>(3).ToList();
    //list.FirstOrDefault().N5 = 3.3232423424234M;
    _testOutputHelper.WriteLine(list.Count.ToString());

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    mapper = ObjectMapperManager.DefaultInstance.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    var tolist = mapper.MapEnum(list);
    using var f = list.GetEnumerator();
    using var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine(f.Current?.N5.ToString());
      var fv = ReflectionHelper.GetPublicFieldsAndProperties(typeof(SimpleTypesSource))
        .Select(m => GetMemberValue(m, f.Current)).Select(m => new { Name = m.Key, FValue = m.Value });

      var tv = ReflectionHelper.GetPublicFieldsAndProperties(typeof(SimpleTypesDestination)).Select(
        m => GetMemberValue(m, t.Current)).Select(m => new { Name = m.Key, TValue = m.Value });

      var result = fv
        .Join(tv, a => a.Name, b => b.Name, (a, b) => new { a.Name, a.FValue, b.TValue });

      foreach (var temp in result)
      {
        Assert.True(Assert.Equal(temp.FValue, temp.TValue), $"Member '{temp.Name} is not equal. Source value£º{temp.FValue}, Destination:{temp.TValue}");
      }
    }
  }
  public static KeyValuePair<string, object> GetMemberValue(MemberInfo member, object target)
  {
    return member switch
    {
      PropertyInfo property => KeyValuePair.Create(property.Name, property.GetValue(target)),
      MethodInfo method => KeyValuePair.Create(method.Name, method.Invoke(target, null)),
      FieldInfo field => KeyValuePair.Create(field.Name, field.GetValue(target)),
      null => throw new ArgumentNullException(nameof(member)),
      _ => throw new ArgumentOutOfRangeException(nameof(member))
    };
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


  internal class RandomDoublePrecisionFloatingPointSequenceGenerator
    : ISpecimenBuilder
  {
    private readonly object syncRoot;
    private readonly Random random;

    internal RandomDoublePrecisionFloatingPointSequenceGenerator()
    {
      syncRoot = new object();
      random = new Random();
    }

    public object Create(object request, ISpecimenContext context)
    {
      var type = request as Type;
      if (type == null)
        return new NoSpecimen();

      return CreateRandom(type);
    }

    private double GetNextRandom()
    {
      lock (syncRoot)
      {
        return random.NextDouble();
      }
    }

    private object CreateRandom(Type request)
    {
      switch (Type.GetTypeCode(request))
      {
        case TypeCode.Decimal:
          return (decimal)
            GetNextRandom();

        case TypeCode.Double:
          return GetNextRandom();

        case TypeCode.Single:
          return (float)
            GetNextRandom();

        default:
          return new NoSpecimen();
      }
    }
  }
}