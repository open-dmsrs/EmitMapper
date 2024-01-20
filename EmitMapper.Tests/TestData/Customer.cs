namespace EmitMapper.Tests.TestData;

/// <summary>
///   The customer.
/// </summary>
public class Customer : ITestObject
{
	/// <summary>
	///   Gets or Sets the address.
	/// </summary>
	public Address? Address { get; set; }
	/// <summary>
	///   Gets or Sets the addresses.
	/// </summary>
	public Address[]? Addresses { get; set; }
	/// <summary>
	///   Gets or Sets the credit.
	/// </summary>
	public decimal? Credit { get; set; }
	/// <summary>
	///   Gets or Sets the home address.
	/// </summary>
	public Address? HomeAddress { get; set; }
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
	public List<Address>? WorkAddresses { get; set; }
}