using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
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
            Source = new(
                new[]
                {
                    typeof(Source).GetMember(nameof(Source.InnerSource))[0],
                    typeof(Source.InnerSourceClass).GetMember(nameof(Source.InnerSource.Message))[
                        0]
                }),
            Destination = new(
                new[] { typeof(Destination).GetMember(nameof(Destination.Message))[0] })
        };
        var rw2 = new ReadWriteSimple
        {
            Source = new(
                new[]
                {
                    typeof(Source).GetMember(nameof(Source.InnerSource))[0],
                    typeof(Source.InnerSourceClass).GetMember(
                        nameof(Source.InnerSourceClass.GetMessage2))[0]
                }),
            Destination = new(
                new[] { typeof(Destination).GetMember(nameof(Destination.Message2))[0] })
        };

        var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>(
            new CustomMapConfig { GetMappingOperationFunc = (from, to) => new IMappingOperation[] { rw1, rw2 } });
        var b = new Source();
        var a = mapper.Map(b);
        Assert.Equal(b.InnerSource.Message, a.Message);
        Assert.Equal(b.InnerSource.GetMessage2(), a.Message2);
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