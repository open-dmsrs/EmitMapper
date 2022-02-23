using System;
using EMConfigurations;
using EmitMapper;
using Shouldly;
using Xunit;

namespace SamplesTests;

public class Flattering
{
  [Fact]
  public void TestFlattering()
  {
    var source = new ModelObject
    {
      BaseDate = DateTime.Now,
      Sub = new ModelSubObject
      {
        ProperName = "Some name", SubSub = new ModelSubSubObject { IAmACoolProperty = "Cool daddy-o" }
      },
      Sub2 = new ModelSubObject { ProperName = "Sub 2 name" },
      SubWithExtraName = new ModelSubObject { ProperName = "Some other name" }
    };

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ModelObject, ModelDto>(
      new FlatteringConfig().IgnoreMembers<ModelObject, ModelDto>("SubSubSubNoExistInSourceProperty"));

    var b = mapper.Map(source);

    b.BaseDate.ShouldBe(source.BaseDate);
    b.TestMethod1.ShouldBe(source.TestMethod1());
    b.SubProperName.ShouldBe(source.Sub.ProperName);
    b.SubSubSubIAmACoolProperty.ShouldBe(source.Sub.SubSub.IAmACoolProperty);
    b.Sub2ProperName.ShouldBe(source.Sub2.ProperName);
    b.SubWithExtraNameProperName.ShouldBe(source.SubWithExtraName.ProperName);
  }

  public class ModelDto
  {
    public string TestMethod1;

    public DateTime BaseDate { get; set; }

    public string Sub2ProperName { get; set; }

    public string SubProperName { get; set; }

    public string SubSubSubIAmACoolProperty { get; set; }

    public string SubWithExtraNameProperName { get; set; }

    /// <summary>
    ///   cant support this property
    /// </summary>

    // public string SubSubSubNoExistInSourceProperty { get; set; }
    public void T()
    {
    }
  }

  public class ModelObject
  {
    public DateTime BaseDate { get; set; }

    public ModelSubObject Sub { get; set; }

    public ModelSubObject Sub2 { get; set; }

    public ModelSubObject SubWithExtraName { get; set; }

    public void TestMethod()
    {
    }

    public string TestMethod1()
    {
      return "1";
    }
  }

  public class ModelSubObject
  {
    public string ProperName { get; set; }

    public ModelSubSubObject SubSub { get; set; }

    public void TestMethod()
    {
    }

    public string TestMethod1()
    {
      return "1";
    }
  }

  public class ModelSubSubObject
  {
    public string IAmACoolProperty { get; set; }
  }
}