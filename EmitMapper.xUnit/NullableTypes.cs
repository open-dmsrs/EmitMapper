namespace EmitMapper.xUnit
{
    using System;

    using EmitMapper.MappingConfiguration;

    using Xunit;

    ////[TestFixture]
    public class NullableTypes
    {
        [Fact]
        public void Nullable_to_Value()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B1, A1>(
                new DefaultMapConfig().NullSubstitution<B1.Int1, A1.Int1>(state => new A1.Int1(0))
                    .NullSubstitution<int?, int>(state => 3).NullSubstitution<int?, int?>(state => 4));
            //DynamicAssemblyManager.SaveAssembly();
            var a = mapper.Map(new B1());
            Assert.Equal(10, a.fld1);
            Assert.NotNull(a.i);
            Assert.Equal("A1::Int1::s", a.i.s);
            Assert.Equal(3, a.fld2);
            Assert.Equal(4, a.fld3);
        }

        [Fact]
        public void Value_to_Nullable()
        {
            var a = Context.objMan.GetMapper<B2, A2>().Map(new B2());
            Assert.Equal(10, a.fld1);
        }

        [Fact]
        public void NullableStruct_to_Struct()
        {
            var bint = new B3.BInt { fld1 = "b" };
            var b = new B3
                        {
                            fld1 = bint,
                            fld2 = bint,
                            fld3 = bint,
                            fld4 = bint,
                            fld6 = bint,
                            fld7 = bint
                        };
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();
            //DynamicAssemblyManager.SaveAssembly();

            var a = mapper.Map(b);
            Assert.Equal("b", a.fld1.Value.fld1);
            Assert.Equal("b", a.fld2.Value.fld1);
            Assert.Equal("b", a.fld3.Value.fld1);
            Assert.Equal("b", a.fld4.Value.fld1);
            Assert.Equal("b", a.fld6.Value.fld1);
            Assert.Equal("b", a.fld7.fld1);
            Assert.Equal("a", a.fld2.Value.fld3);
            Assert.False(a.fld5.HasValue);
        }

        [Fact]
        public void Struct_to_NullableStruct()
        {
            var bint = new B4.BInt { fld1 = "b" };
            var b = new B4 { fld1 = bint };
            var a = Context.objMan.GetMapper<B4, A4>().Map(b);
            Assert.Equal("b", a.fld1.fld1);
        }

        [Fact]
        public void Test_Nullable()
        {
            var a = Context.objMan.GetMapper<B5, A5>().Map(new B5());
            Assert.Equal(10, a.fld1.Value);
            Assert.Null(a.fld2);
            Assert.Equal(A5.En.value2, a.fld3.Value);
            Assert.Equal(A5.En.value3, a.fld4);
            Assert.Equal(13, a.fld5.Value);
            Assert.Equal(11, a.fld6.Value);
        }

        [Fact]
        public void Test_Object_Nullable()
        {
            var a = ObjectMapperManager.DefaultInstance
                .GetMapper<B6, A6>(new DefaultMapConfig().DeepMap().ConvertUsing<object, object>(v => null))
                .Map(new B6());
            Assert.Null(a);
        }

        [Fact]
        public void Test_Object_Nullable7()
        {
            var a = ObjectMapperManager.DefaultInstance
                .GetMapper<B7, A7>(new DefaultMapConfig().DeepMap().ConvertUsing<object, int>(v => 100)).Map(new B7());

            Assert.Equal(100, a.i);
        }

        public class A1
        {
            public int fld1;

            public int fld2;

            public int? fld3;

            public Int1 i;

            public class Int1
            {
                public string s;

                public Int1()
                {
                }

                public Int1(int i)
                {
                    this.s = "A1::Int1::s";
                }
            }
        }

        public class B1
        {
            public int? fld1 = 10;

            public int? fld2;

            public int? fld3;

            public Int1 i;

            public class Int1
            {
                public string s = "B1::Int1::s";
            }
        }

        public class A2
        {
            public int? fld1;
        }

        public class B2
        {
            public int fld1 = 10;
        }

        public class A3
        {
            public AInt? fld1;

            public AInt? fld4;

            public AInt? fld5;

            public AInt? fld6;

            public AInt fld7;

            public A3()
            {
                var a = new AInt { fld3 = "a" };
                this.fld2 = a;
                this.fld5 = a;
            }

            public AInt? fld2 { get; set; }

            public AInt? fld3 { get; set; }

            public struct AInt
            {
                public string fld1;

                public string fld2;

                public string fld3;
            }
        }

        public class B3
        {
            public BInt? fld1;

            public BInt? fld3;

            public BInt? fld5;

            public BInt fld6;

            public BInt? fld7;

            public BInt? fld2 { get; set; }

            public BInt? fld4 { get; set; }

            public struct BInt
            {
                public string fld1;

                public string fld2;
            }
        }

        public class A4
        {
            public AInt fld1;

            public struct AInt
            {
                public string fld1;
            }
        }

        public class B4
        {
            public BInt? fld1 = new BInt();

            public struct BInt
            {
                public string fld1;
            }
        }

        public class A5
        {
            public enum En
            {
                value1,

                value2,

                value3
            }

            public int? fld1 = 0;

            public int? fld2 = 10;

            public En? fld3 = En.value1;

            public En fld4 = En.value1;

            public int? fld5 = 0;

            public int? fld6 = null;
        }

        public class B5
        {
            public enum En
            {
                value1,

                value2,

                value3
            }

            public int fld1 = 10;

            public string fld2 = null;

            public En fld3 = En.value2;

            public En? fld4 = En.value3;

            public int? fld5 = 13;

            public string fld6 = "11";
        }

        public class A6
        {
            public int? i { get; set; }

            public DateTime? dt { get; set; }
        }

        public class B6
        {
        }

        public class A7
        {
            public int? i { get; set; }
        }

        public class B7
        {
            public decimal i = 100;
        }
    }
}