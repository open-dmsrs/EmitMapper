namespace EmitMapper.Tests.TestData;

/// <summary>
///   The customer d t o.
/// </summary>
public class CustomerDto : ITestObject
{
	/// <summary>
	///   Gets or Sets the address.
	/// </summary>
	public Address? Address { get; set; }
	/// <summary>
	///   Gets or Sets the address city.
	/// </summary>
	public string? AddressCity { get; set; }
	/// <summary>
	///   Gets or Sets the addresses.
	/// </summary>
	public AddressDto[]? Addresses { get; set; }
	/// <summary>
	///   Gets or Sets the home address.
	/// </summary>
	public AddressDto? HomeAddress { get; set; }
	/// <summary>
	///   Gets or Sets the id.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	///   Gets or Sets the name.
	/// </summary>
	public string? Name { get; set; }
	/// <summary>
	///   Gets or Sets the work addresses.
	/// </summary>
	public List<AddressDto>? WorkAddresses { get; set; }
}