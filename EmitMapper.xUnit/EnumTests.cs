using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class EnumTests
{
    public enum En1 : byte
    {
        A = 1,

        B = 2,

        C = 3
    }

    public enum En2 : long
    {
        A = 1,

        B = 2,

        C = 3
    }

    public enum En3
    {
        B = 2,

        C = 3,

        A = 1
    }

    [Fact]
    public void EnumTests1()
    {
        var mapper = Context.ObjMan.GetMapper<B, A>();
        //DynamicAssemblyManager.SaveAssembly();

        var a = mapper.Map(new());
        Assert.True(a.En1 == En1.C);
        Assert.True(a.En2 == En2.C);
        Assert.True(a.En3 == En3.C);
        Assert.True(a.En4 == 2);
        Assert.True(a.En6 == En1.C);
        Assert.True(a.En7 == En3.C);
        Assert.True(a.En8 == En3.C);
        Assert.Null(a.En9);
    }

    public class A
    {
        public decimal En4;

        public En1? En6;
        public En2 En2;

        public En3 En7;

        public En3? En8;

        public En3? En9 = En3.C;

        public string En5;

        public En1 En1 { get; set; }

        public En3 En3 { get; set; }
    }

    public class B
    {
        public decimal En1 = 3;

        public En1? En7 = EnumTests.En1.C;

        public En1? En8 = EnumTests.En1.C;

        public En2 En4 = EnumTests.En2.B;

        public En2 En6 = EnumTests.En2.C;

        public En2? En9 = null;

        public En3 En5 = EnumTests.En3.A;

        public string En3 = "C";

        public B()
        {
            En2 = EnumTests.En1.C;
        }

        public En1 En2 { get; set; }
    }
}