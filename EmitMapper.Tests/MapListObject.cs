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

namespace EmitMapper.Tests;

/// <summary>
///   The map list object.
/// </summary>
public class MapListObject
{
  private readonly ITestOutputHelper _testOutputHelper;

  /// <summary>
  ///   Initializes a new instance of the <see cref="MapListObject" /> class.
  /// </summary>
  /// <param name="testOutputHelper">The test output helper.</param>
  public MapListObject(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  /// <summary>
  ///   Converts the char to int32.
  /// </summary>
  [Fact]
  public void ConvertCharToInt32()
  {
    var m = 'a';
    var n = Convert.ToInt32(m);

    _testOutputHelper.WriteLine(n + string.Empty);
  }

  /// <summary>
  ///   Test_s the emit mapper_ map_ list object.
  /// </summary>
  /// <param name="listFrom">The list from.</param>
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

    var mapper = Mapper.Default.GetMapper<List<FromClass>, List<ToClass>>(
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

  /// <summary>
  ///   Test_s the emit mapper_ map enum.
  /// </summary>
  [Fact]
  public void Test_EmitMapper_MapEnum()
  {
    Fixture fixture = new();

    // fixture.Customizations.Add(
    // new RandomDoublePrecisionFloatingPointSequenceGenerator());
    var list = fixture.CreateMany<SimpleTypesSource>(3).ToList();

    // list.FirstOrDefault().N5 = 3.3232423424234M;
    _testOutputHelper.WriteLine(list.Count.ToString());

    var mapper = Mapper.Default.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    mapper = Mapper.Default.GetMapper<SimpleTypesSource, SimpleTypesDestination>();
    var tolist = mapper.MapEnum(list);

    // tolist.ShouldBe(list);
    IsSame(list, tolist);
  }

  /// <summary>
  ///   Gets the member value.
  /// </summary>
  /// <param name="member">The member.</param>
  /// <param name="target">The target.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  /// <returns><![CDATA[KeyValuePair<string, object>]]></returns>
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

  /// <summary>
  /// </summary>
  /// <param name="sources">The sources.</param>
  /// <param name="destinations">The destinations.</param>
  private static void IsSame(IEnumerable<SimpleTypesSource> sources, IEnumerable<SimpleTypesDestination> destinations)
  {
    using var f = sources.GetEnumerator();
    using var t = destinations.GetEnumerator();
    while (f.MoveNext() && t.MoveNext()) IsSame(f.Current, t.Current);
  }

  /// <summary>
  /// </summary>
  /// <param name="source">The source.</param>
  /// <param name="destination">The destination.</param>
  private static void IsSame(SimpleTypesSource source, SimpleTypesDestination destination)
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

  /// <summary>
  ///   Test_s the emit mapper_ map_ array list_ nested fields.
  /// </summary>
  /// <param name="list">The list.</param>
  [Theory]
  [AutoData]
  public void Test_EmitMapper_Map_ArrayList_NestedFields(List<FromClass> list)
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

    var mapper = Mapper.Default.GetMapper<ArrayList, ArrayList>(
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

  /// <summary>
  ///   The from class.
  /// </summary>
  public class FromClass
  {
    public InnerClass Inner = new();

    /// <summary>
    ///   The inner class.
    /// </summary>
    public class InnerClass
    {
      public string Message = "hello";

      /// <summary>
      ///   Gets the message2.
      /// </summary>
      /// <returns>A string.</returns>
      public string GetMessage2()
      {
        return "medved";
      }
    }
  }

  /// <summary>
  ///   The random double precision floating point sequence generator.
  /// </summary>
  internal class RandomDoublePrecisionFloatingPointSequenceGenerator : ISpecimenBuilder
  {
    private readonly Random random;
    private readonly object syncRoot;

    /// <summary>
    ///   Initializes a new instance of the <see cref="RandomDoublePrecisionFloatingPointSequenceGenerator" /> class.
    /// </summary>
    internal RandomDoublePrecisionFloatingPointSequenceGenerator()
    {
      syncRoot = new object();
      random = new Random();
    }

    /// <summary>
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="context">The context.</param>
    /// <returns>An object.</returns>
    public object Create(object request, ISpecimenContext context)
    {
      var type = request as Type;

      if (type == null)
        return new NoSpecimen();

      return CreateRandom(type);
    }

    /// <summary>
    ///   Creates the random.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>An object.</returns>
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

    /// <summary>
    ///   Gets the next random.
    /// </summary>
    /// <returns>A double.</returns>
    private double GetNextRandom()
    {
      lock (syncRoot)
      {
        return random.NextDouble();
      }
    }
  }

  /// <summary>
  ///   The to class.
  /// </summary>
  public class ToClass
  {
    public string Message;
    public string Message2;
  }
}