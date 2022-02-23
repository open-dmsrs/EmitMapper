using System.Collections.Generic;

namespace EmitMapper.Tests.TestData;

public class CustomerDTO : ITestObject
{
  public Address Address { get; set; }
  public string AddressCity { get; set; }
  public AddressDTO[] Addresses { get; set; }
  public AddressDTO HomeAddress { get; set; }
  public int Id { get; set; }
  public string Name { get; set; }
  public List<AddressDTO> WorkAddresses { get; set; }
}