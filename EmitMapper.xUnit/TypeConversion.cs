namespace EmitMapper.xUnit
{
    using EmitMapper.MappingConfiguration;

    using Xunit;

    ////[TestFixture]
    public class TypeConversion
    {
        [Fact]
        public void Test1()
        {
            var a = new A1();
            var b = new B1();
            var mapper = Context.objMan.GetMapper<B1, A1>();
            //DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.Equal(15, a.fld1);
            Assert.Equal("11", a.fld2);
        }

        [Fact]
        public void Test2()
        {
            var a = new A2();
            var b = new B2();
            Context.objMan.GetMapper<B2, A2>().Map(b, a);
            //DynamicAssemblyManager.SaveAssembly();
            Assert.Equal("99", a.fld3[0]);
        }

        [Fact]
        public void Test3_ShallowCopy()
        {
            var a = new A3();
            var b = new B3();
            b.a1.fld1 = 15;
            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().DeepMap()).Map(b, a);
            Assert.Equal(15, a.a1.fld1);
            b.a1.fld1 = 666;
            Assert.Equal(15, a.a1.fld1);

            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap()).Map(b, a);
            b.a1.fld1 = 777;
            Assert.Equal(777, a.a1.fld1);

            b = new B3();
            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap<A1>().DeepMap<A2>()).Map(b, a);
            b.a1.fld1 = 333;
            b.a2.fld3 = new string[1];

            Assert.Equal(333, a.a1.fld1);
            Assert.Null(a.a2.fld3);
        }

        [Fact]
        public void Test4()
        {
            var a = new A4();
            var b = new B4();
            Context.objMan.GetMapper<B4, A4>().Map(b, a);
            Assert.Equal("string", a.str);
        }

        public class A1
        {
            public int fld1;

            public string fld2;
        }

        public class B1
        {
            public decimal fld1 = 15;

            public decimal fld2 = 11;
        }

        public class A2
        {
            public string[] fld3;
        }

        public class B2
        {
            public int fld3 = 99;
        }

        public class A3
        {
            public A1 a1 = new A1();

            public A2 a2 = new A2();
        }

        public class B3
        {
            public A1 a1 = new A1();

            public A2 a2 = new A2();
        }

        public class A4
        {
            public string str { set; get; }
        }

        public class B4
        {
            public BInt str { get; } = new BInt();

            public class BInt
            {
                public override string ToString()
                {
                    return "string";
                }
            }
        }

        public class A5
        {
            public string fld2;
        }

        public class B5
        {
            public decimal fld2 = 11;
        }
    }
}