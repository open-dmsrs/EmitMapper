using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class Flattering
{
  [Fact]
  public void TestFlattering1()
  {
    var rw1 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(Source).GetMember(nameof(Source.InnerSource))[0],
          typeof(Source.InnerSourceClass).GetMember(nameof(Source.InnerSource.Message))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(Destination).GetMember(nameof(Destination.Message))[0] })
    };

    var rw2 = new ReadWriteSimple
    {
      Source = new MemberDescriptor(
        new[]
        {
          typeof(Source).GetMember(nameof(Source.InnerSource))[0],
          typeof(Source.InnerSourceClass).GetMember(nameof(Source.InnerSourceClass.GetMessage2))[0]
        }),
      Destination = new MemberDescriptor(
        new[] { typeof(Destination).GetMember(nameof(Destination.Message2))[0] })
    };

    var mapper = Mapper.Default.GetMapper<Source, Destination>(
      new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

    var b = new Source();
    var result = mapper.Map(b);
    b.InnerSource.Message.ShouldBe(result.Message);
    b.InnerSource.GetMessage2().ShouldBe(result.Message2);
  }

  public class Destination
  {
    public string Message;
    public string Message2;
  }

  public class Source
  {
    public InnerSourceClass InnerSource = new();

    public class InnerSourceClass
    {
      public string Message = "message's value";

      public string GetMessage2()
      {
        return "GetMessage2 's value";
      }
    }
  }
}