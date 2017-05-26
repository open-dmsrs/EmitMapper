using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EmitMapper.Utils;
using Xunit;

namespace EmitMapperTests.NUnit
{
    //////[TestFixture]
    public class ReflectionUtilsTests
    {

        public interface IBase
        {
            string BaseProperty { get; set; }
        }

        public interface IDerived : IBase
        {
            string DerivedProperty { get; set; }
        }

        [Fact]
        public void Test_GetPublicFieldsAndProperties_ShouldIncludeMembersFromAllInterfaces()
        {
            MemberInfo[] members = ReflectionUtils.GetPublicFieldsAndProperties(typeof (IDerived));
            Assert.Equal(2, members.Count());
        }
    }
}
