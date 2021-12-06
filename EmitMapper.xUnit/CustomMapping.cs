namespace EmitMapperTests
{
    using EmitMapper.MappingConfiguration;

    using Xunit;

    ////[TestFixture]
    public class CustomMapping
    {
        [Fact]
        public void Test_CustomConverterWithInterfaces()
        {
            var str = Context.objMan.GetMapper<WithName, string>(
                    new DefaultMapConfig().ConvertUsing<IWithName, string>(v => v.Name).SetConfigName("withinterfaces"))
                .Map(new WithName { Name = "thisIsMyName" });

            Assert.Equal("thisIsMyName", str);
        }

        [Fact]
        public void Test_CustomConverter()
        {
            var a = Context.objMan.GetMapper<B2, A2>(
                new DefaultMapConfig().ConvertUsing<object, string>(v => "333")
                    .ConvertUsing<object, string>(v => "hello").SetConfigName("ignore")).Map(new B2());
            Assert.Null(a.fld1);
            Assert.Equal("hello", a.fld2);

            a = Context.objMan.GetMapper<B2, A2>().Map(new B2());
            Assert.Equal("B2::fld2", a.fld2);
        }

        [Fact]
        public void Test_CustomConverter2()
        {
            var a = Context.objMan.GetMapper<BB, AA>(
                new DefaultMapConfig().ConvertUsing<object, string>(v => "converted " + v)).Map(new BB());
            Assert.Equal("converted B2::fld1", a.fld1);
            Assert.Equal("converted B2::fld2", a.fld2);
        }

        [Fact]
        public void Test_PostProcessing()
        {
            var a = Context.objMan.GetMapper<B3, A3>(
                new DefaultMapConfig().PostProcess<A3.Int>(
                    (i, state) =>
                        {
                            i.str2 = "processed";
                            return i;
                        }).PostProcess<A3.SInt?>(
                    (i, state) => { return new A3.SInt { str1 = i.Value.str1, str2 = "processed" }; }).PostProcess<A3>(
                    (i, state) =>
                        {
                            i.status = "processed";
                            return i;
                        })).Map(new B3());
            Assert.Equal("B3::Int::str1", a.fld.str1);
            Assert.Equal("processed", a.fld.str2);

            Assert.Equal("B3::SInt::str1", a.fld2.Value.str1);
            Assert.Equal("processed", a.fld2.Value.str2);

            Assert.Equal("processed", a.status);
        }

        public class A1
        {
            public string fld1 = "";

            public string fld2 { get; private set; } = "";

            public void SetFld2(string value)
            {
                this.fld2 = value;
            }
        }

        public class A2
        {
            public string fld1;

            public string fld2;
        }

        public class B2
        {
            public string fld2 = "B2::fld2";

            public string fld3 = "B2::fld3";
        }

        public interface IWithName
        {
            string Name { get; set; }
        }

        public class WithName : IWithName
        {
            public string Name { get; set; }
        }

        public class AA
        {
            public string fld1;

            public string fld2;
        }

        public class BB
        {
            public string fld1 = "B2::fld1";

            public string fld2 = "B2::fld2";
        }

        public class A3
        {
            public Int fld;

            public SInt? fld2;

            public string status;

            public class Int
            {
                public string str1;

                public string str2;
            }

            public struct SInt
            {
                public string str1;

                public string str2;
            }
        }

        public class B3
        {
            public Int fld = new Int();

            public SInt fld2;

            public B3()
            {
                this.fld2.str1 = "B3::SInt::str1";
            }

            public class Int
            {
                public string str1 = "B3::Int::str1";
            }

            public struct SInt
            {
                public string str1;
            }
        }
    }
}