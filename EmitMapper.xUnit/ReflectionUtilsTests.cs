using System.Linq;
using EmitMapper.Utils;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

//////[TestFixture]
public class ReflectionUtilsTests
{
  [Fact]
  public void Test_GetPublicFieldsAndProperties_ShouldIncludeMembersFromAllInterfaces()
  {
    var members = ReflectionHelper.GetPublicFieldsAndProperties(typeof(IDerived));
    members.Count().ShouldBe(2);
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