using System.Linq;
using EmitMapper.Utils;
using Xunit;

namespace EmitMapper.Tests;

//////[TestFixture]
public class ReflectionUtilsTests
{
  [Fact]
  public void Test_GetPublicFieldsAndProperties_ShouldIncludeMembersFromAllInterfaces()
  {
    var members = ReflectionUtils.GetPublicFieldsAndProperties(typeof(IDerived));
    Assert.Equal(2, members.Count());
  }

  public interface IBase
  {
    string BaseProperty { get; set; }
  }

  public interface IDerived : IBase
  {
    string DerivedProperty { get; set; }
  }
}