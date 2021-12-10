namespace EmitMapper.xUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EmitMapper;
    using EmitMapper.MappingConfiguration;
    using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

    using Xunit;

    ////[TestFixture]
    public class IgnoreByAttributes
    {
        [Fact]
        public void Test()
        {
            var mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<IgnoreByAttributesSrc, IgnoreByAttributesDst>(
                    new MyConfigurator());
            var dst = mapper.Map(new IgnoreByAttributesSrc());
            Assert.Equal("IgnoreByAttributesDst::str1", dst.str1);
            Assert.Equal("IgnoreByAttributesSrc::str2", dst.str2);
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class MyIgnoreAttribute : Attribute
        {
        }

        public class IgnoreByAttributesSrc
        {
            [MyIgnore]
            public string str1 = "IgnoreByAttributesSrc::str1";

            public string str2 = "IgnoreByAttributesSrc::str2";
        }

        public class IgnoreByAttributesDst
        {
            public string str1 = "IgnoreByAttributesDst::str1";

            public string str2 = "IgnoreByAttributesDst::str2";
        }

        public class MyConfigurator : DefaultMapConfig
        {
            public override IMappingOperation[] GetMappingOperations(Type from, Type to)
            {
                this.IgnoreMembers<object, object>(
                    this.GetIgnoreFields(from).Concat(this.GetIgnoreFields(to)).ToArray());
                return base.GetMappingOperations(from, to);
            }

            private IEnumerable<string> GetIgnoreFields(Type type)
            {
                return type.GetFields().Where(f => f.GetCustomAttributes(typeof(MyIgnoreAttribute), false).Any())
                    .Select(f => f.Name).Concat(
                        type.GetProperties().Where(p => p.GetCustomAttributes(typeof(MyIgnoreAttribute), false).Any())
                            .Select(p => p.Name));
            }
        }
    }
}