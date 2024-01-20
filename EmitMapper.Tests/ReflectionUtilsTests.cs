namespace EmitMapper.Tests;

/// <summary>
///   The reflection utils tests.
/// </summary>
public class ReflectionUtilsTests
{
	/// <summary>
	///   Test_s the get public fields and properties_ should include members from all interfaces.
	/// </summary>
	[Fact]
	public void TestGetPublicFieldsAndPropertiesShouldIncludeMembersFromAllInterfaces()
	{
		var members = ReflectionHelper.GetPublicFieldsAndProperties(typeof(IDerived));
		members.Count().ShouldBe(2);
	}

	/// <summary>
	///   The base interface.
	/// </summary>
	public interface IBase
	{
		/// <summary>
		///   Gets or Sets the base property.
		/// </summary>
		string BaseProperty { get; set; }
	}

	/// <summary>
	///   The derived interface.
	/// </summary>
	public interface IDerived : IBase
	{
		/// <summary>
		///   Gets or Sets the derived property.
		/// </summary>
		string DerivedProperty { get; set; }
	}
}