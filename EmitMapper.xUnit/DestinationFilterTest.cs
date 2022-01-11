using EmitMapper.MappingConfiguration;
using Xunit;

namespace EmitMapper.Tests;

public class DestinationFilterTest
{
  [Fact]
  public void Test_Derived()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<IDerived, Target>();

    var source = new Derived { BaseProperty = "base", DerivedProperty = "derived" };

    var destination = mapper.Map(source);
    Assert.Equal("base", destination.BaseProperty);
    Assert.Equal("derived", destination.DerivedProperty);
  }

  [Fact]
  public void TestdestinationFilter()
  {
    var mapper =
      ObjectMapperManager.DefaultInstance.GetMapper<DestinationTestFilterSrc, DestinationTestFilterDest>(
        new DefaultMapConfig().FilterDestination<string>((value, state) => false)
          .FilterDestination<int>((value, state) => value >= 0)
          .FilterSource<int>((value, state) => value >= 10).FilterSource<object>(
            (value, state) =>
              value is not long && (value is not DestinationTestFilterSrc
                                    || (value as DestinationTestFilterSrc).I1 != 666)));
    var dest = mapper.Map(new DestinationTestFilterSrc());

    Assert.Equal(13, dest.I1);
    Assert.Equal(-5, dest.I2);
    Assert.Equal(0, dest.I3);
    Assert.Equal(0, dest.L1);
    Assert.Null(dest.Str);

    dest = mapper.Map(new DestinationTestFilterSrc { I1 = 666 }, new DestinationTestFilterDest());
    Assert.Equal(0, dest.I1);
  }

  [Fact]
  public void TestdestinationFilter1()
  {
    var mapper =
      ObjectMapperManager.DefaultInstance.GetMapper<DestinationTestFilterSrc, DestinationTestFilterDest>(
        new DefaultMapConfig().FilterDestination<string>((value, state) => false)
          .FilterDestination<int>((value, state) => value >= 0)
          .FilterSource<int>((value, state) => value >= 10).FilterSource<object>(
            (value, state) =>
              value is not long && (value is not DestinationTestFilterSrc
                                    || (value as DestinationTestFilterSrc).I1 != 666)));
    var dest = mapper.Map(new DestinationTestFilterSrc());

    Assert.Equal(13, dest.I1);
    Assert.Equal(-5, dest.I2);
    Assert.Equal(0, dest.I3);
    Assert.Equal(0, dest.L1);
    Assert.Null(dest.Str);

    dest = mapper.Map(new DestinationTestFilterSrc { I1 = 666 }, new DestinationTestFilterDest());
    Assert.Equal(0, dest.I1);
  }

  [Fact]
  public void TestInheritence()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<BaseSource, InherDestination>();
    var dest = mapper.Map(new DerivedSource { I1 = 1, I2 = 2, I3 = 3, I4 = 4 });

    Assert.Equal(1, dest.I1);
    Assert.Equal(2, dest.I2);
    Assert.Equal(3, dest.I3);
  }

  public class DestinationTestFilterDest
  {
    public int I1;

    public int I2 = -5;

    public int I3 = 0;

    public long L1;

    public string Str;
  }

  public interface IBase
  {
    string BaseProperty { get; set; }
  }

  public interface IDerived : IBase
  {
    string DerivedProperty { get; set; }
  }

  public class Derived : IDerived
  {
    #region Implementation of IBase

    public string BaseProperty { get; set; }

    #endregion

    #region Implementation of IDerived

    public string DerivedProperty { get; set; }

    #endregion
  }

  public class DestinationTestFilterSrc
  {
    public int I1 = 13;

    public int I2 = 14;

    public int I3 = 5;

    public long L1 = 5;

    public string Str = "hello";
  }

  public class Target
  {
    public string BaseProperty { get; set; }

    public string DerivedProperty { get; set; }
  }

  public class BaseSource
  {
    public int I1;

    public int I2;

    public int I3;
  }

  public class DerivedSource : BaseSource
  {
    public int I4;
  }

  public class InherDestination
  {
    public int I1;

    public int I2;

    public int I3;
  }
}