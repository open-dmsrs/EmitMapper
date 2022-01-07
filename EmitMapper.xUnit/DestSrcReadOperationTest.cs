using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;

public class DestSrcReadOperationTest
{
  private readonly ITestOutputHelper _testOutputHelper;

  public DestSrcReadOperationTest(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public void TestDestSrcReadOperation()
  {
    var message2 = new ReadWriteSimple
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
    var message = new DestSrcReadOperation
    {
      ValueProcessor = (f, t, o) =>
      {
        _testOutputHelper.WriteLine(f?.ToString());
        _testOutputHelper.WriteLine(t?.ToString());
        // _testOutputHelper.WriteLine(o?.ToString());
        // var source = f as FromClass;
        // var dest = t as ToClass;
        // dest.Message = source.Inner.Message;
      },

      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.Inner.Message))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(ToClass).GetMember(nameof(ToClass.Message))[0] })
    };
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<FromClass, ToClass>(
      new CustomMapConfig
      {
        GetMappingOperationFunc = (from, to) => new IMappingOperation[] { message, message2 }
      });
    var b = new FromClass();
    var a = mapper.Map(b);
    // Assert.Equal(b.Inner.Message, a.Message);
    // Assert.Equal(b.Inner.GetMessage2(), a.Message2);
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