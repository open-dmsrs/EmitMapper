using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;

/// <summary>
///   The dest src read operation test.
/// </summary>
public class DestSrcReadOperationTest
{
  private readonly ITestOutputHelper _testOutputHelper;

  /// <summary>
  ///   Initializes a new instance of the <see cref="DestSrcReadOperationTest" /> class.
  /// </summary>
  /// <param name="testOutputHelper">The test output helper.</param>
  public DestSrcReadOperationTest(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  /// <summary>
  ///   Tests the dest src read operation.
  /// </summary>
  [Fact]
  public void TestDestSrcReadOperation()
  {
    var message2 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(FromClass).GetMember(nameof(FromClass.Inner))[0],
          typeof(FromClass.InnerClass).GetMember(nameof(FromClass.InnerClass.GetMessage2))[0]
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

    var mapper = Mapper.Default.GetMapper<FromClass, ToClass>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => new IMappingOperation[] { message, message2 } });

    var b = new FromClass();
    var a = mapper.Map(b);

    // b.Inner.Message.ShouldBe(a.Message);
    // b.Inner.GetMessage2().ShouldBe(a.Message2);
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
  ///   The to class.
  /// </summary>
  public class ToClass
  {
    public string Message;
    public string Message2;
  }
}