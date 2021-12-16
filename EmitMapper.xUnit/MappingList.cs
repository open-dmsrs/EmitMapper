using System.Collections.Generic;
using AutoFixture.Xunit2;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
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


        var mapper = ObjectMapperManager.DefaultInstance.GetMapper<List<FromClass>, List<ToClass>>(
            new CustomMapConfig
            {
                GetMappingOperationFunc = (from, to) => new IMappingOperation[] { rw1, rw2 }
            });

        var tolist = mapper.Map(listFrom);
        using (var f = listFrom.GetEnumerator())
        {
            using (var t = tolist.GetEnumerator())
            {
                while (f.MoveNext() && t.MoveNext())
                {
                    _testOutputHelper.WriteLine(t.Current.Message);
                    Assert.Equal(f.Current.Inner.Message, t.Current.Message);
                    Assert.Equal(f.Current.Inner.GetMessage2(), t.Current.Message2);
                }
            }
        }
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