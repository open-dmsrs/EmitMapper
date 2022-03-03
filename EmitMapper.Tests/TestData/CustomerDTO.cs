using System.Collections.Generic;

namespace EmitMapper.Tests.TestData;
/// <summary>
/// The customer d t o.
/// </summary>

public class CustomerDTO : ITestObject
{
  /// <summary>
  /// Gets or Sets the address.
  /// </summary>
  public Address Address { get; set; }
  /// <summary>
  /// Gets or Sets the address city.
  /// </summary>
  public string AddressCity { get; set; }
  /// <summary>
  /// Gets or Sets the addresses.
  /// </summary>
  public AddressDTO[] Addresses { get; set; }
  /// <summary>
  /// Gets or Sets the home address.
  /// </summary>
  public AddressDTO HomeAddress { get; set; }
  /// <summary>
  /// Gets or Sets the id.
  /// </summary>
  public int Id { get; set; }
  /// <summary>
  /// Gets or Sets the name.
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// Gets or Sets the work addresses.
  /// </summary>
  public List<AddressDTO> WorkAddresses { get; set; }
}