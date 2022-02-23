namespace EmitMapper.Tests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;

using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Tests.TestData;
using EmitMapper.Utils;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

public class MapListObject
{
  private readonly ITestOutputHelper _testOutputHelper;

  public MapListObject(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  public static void Equal(IEnumerable<SimpleTypesSource> sources, IEnumerable<SimpleTypesDestination> destinations)
  {
    using var f = sources.GetEnumerator();
    using var t = destinations.GetEnumerator();
    while (f.MoveNext() && t.MoveNext()) Equal(f.Current, t.Current);
  }

  public static void Equal(SimpleTypesSource source, SimpleTypesDestination destination)
  {
    var fv = ReflectionHelper.GetPublicFieldsAndProperties(typeof(SimpleTypesSource))
      .Select(m => GetMemberValue(m, source)).Select(m => new { Name = m.Key, FValue = m.Value });

    var tv = ReflectionHelper.GetPublicFieldsAndProperties(typeof(SimpleTypesDestination))
      .Select(m => GetMemberValue(m, destination)).Select(m => new { Name = m.Key, TValue = m.Value });

    var result = fv.Join(tv, a => a.Name, b => b.Name, (a, b) => new { a.Name, a.FValue, b.TValue });

    foreach (var temp in result)
      if (temp.Name == "N8")
        Assert.True(
          Convert.ToInt32(temp.FValue) == Convert.ToInt32(temp.TValue),
          $"Member '{temp.Name} is not equal. Source value£º{temp.FValue}, Destination:{temp.TValue}");
      else
        Assert.True(
          Convert.ToString(temp.FValue) == Convert.ToString(temp.TValue),
          $"Member '{temp.Name} is not equal. Source value£º{temp.FValue}, Destination:{temp.TValue}");
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

  [Fact]
  public void ConvertCharToInt32()
  {
    var m = 'a';
    var n = Convert.ToInt32(m);

    _testOutputHelper.WriteLine(n + string.Empty);
  }

  [Theory]
  [AutoData]
  public void Test_EmitMapper_Map_ListObject(List<FromClass> listFrom)
  {
    _testOutputHelper.WriteLine(listFrom.Count.ToString());

    var rw1 = new ReadWriteSimple
                {
                  Source = new MemberDescriptor(
                    typeof(FromClass).GetMember(nameof(FromClass.Inner))[0].AsEnumerable(
                      typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[0])),
                  Destination = new MemberDescriptor(
                    typeof(ToClass).GetMember(nameof(ToClass.Message))[0].AsEnumerable())
                };

    var rw2 = new ReadWriteSimple
                {
                  Source = new MemberDescriptor(
                    typeof(FromClass).GetMember(nameof(FromClass.Inner))[0].AsEnumerable(
                      typeof(FromClass.InnerClass).GetMember(nameof(FromClass.InnerClass.GetMessage2))[0])),
                  Destination = new MemberDescriptor(
                    typeof(ToClass).GetMember(nameof(ToClass.Message2))[0].AsEnumerable())
                };

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<List<FromClass>, List<ToClass>>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

    var tolist = mapper.Map(listFrom);
    using var f = listFrom.GetEnumerator();
    using var t = tolist.GetEnumerator();
    while (f.MoveNext() && t.MoveNext())
    {
      _testOutputHelper.WriteLine(t.Current.Message);
      f.Current.Inner.Message.ShouldBe(t.Current.Message);
      f.Current.Inner.GetMessage2().ShouldBe(t.Current.Message2);
    }
  }

  [Fact]
  public void Test_EmitMapper_MapEnum()
  {
    Fixture fixture = new();

    // fixture.Customizations.Add(
    // new RandomDoublePrecisionFloatingPointSequenceGenerator());
    var list = fixture.CreateMany<SimpleTypesSource>(3).ToList();

    // list.FirstOrDefault().N5 = 3.3232423424234M;
    _testOutputHelper.WriteLine(list.Count.ToString());

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    mapper = ObjectMapperManager.DefaultInstance.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    var tolist = mapper.MapEnum(list);

    // tolist.ShouldBe(list);
    Equal(list, tolist);
  }

  // [Theory]
  [AutoData]
  private void Test_EmitMapper_Map_ArrayList_NestedFields(List<FromClass> list)
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
                  Destination = new MemberDescriptor(new[] { typeof(ToClass).GetMember(nameof(ToClass.Message))[0] })
                };

    var rw2 = new ReadWriteSimple
                {
                  Source = new MemberDescriptor(
                    new[]
                      {
                        typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
                        typeof(FromClass.InnerClass).GetMember(nameof(FromClass.InnerClass.GetMessage2))[0]
                      }),
                  Destination = new MemberDescriptor(new[] { typeof(ToClass).GetMember(nameof(ToClass.Message2))[0] })
                };

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ArrayList, ArrayList>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

    var tolist = mapper.Map(listFrom);
    tolist.ShouldBe(listFrom);

    // var f = listFrom.GetEnumerator();
    // var t = tolist.GetEnumerator();
    // while (f.MoveNext() && t.MoveNext())
    // {
    // _testOutputHelper.WriteLine((t.Current as ToClass)?.Message);
    // Assert.Equal(((FromClass)f.Current)?.Inner.Message, (t.Current as ToClass)?.Message);
    // Assert.Equal(((FromClass)f.Current)?.Inner.GetMessage2(), (t.Current as ToClass)?.Message2);
    // }
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

  public class ToClass
  {
    public string Message;

    public string Message2;
  }

  internal class RandomDoublePrecisionFloatingPointSequenceGenerator : ISpecimenBuilder
  {
    private readonly Random random;

    private readonly object syncRoot;

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

    private object CreateRandom(Type request)
    {
      switch (Type.GetTypeCode(request))
      {
        case TypeCode.Decimal:
          return (decimal)GetNextRandom();

        case TypeCode.Double:
          return GetNextRandom();

        case TypeCode.Single:
          return (float)GetNextRandom();

        default:
          return new NoSpecimen();
      }
    }

    private double GetNextRandom()
    {
      lock (syncRoot)
      {
        return random.NextDouble();
      }
    }
  }
}