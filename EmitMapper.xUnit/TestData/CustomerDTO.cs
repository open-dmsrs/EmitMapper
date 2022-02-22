namespace EmitMapper.Tests.TestData;

using System.Collections.Generic;

public class CustomerDTO : ITestObject
{
  public int Id { get; set; }
  public string Name { get; set; }
  public Address Address { get; set; }
  public AddressDTO HomeAddress { get; set; }
  public AddressDTO[] Addresses { get; set; }
  public List<AddressDTO> WorkAddresses { get; set; }
  public string AddressCity { get; set; }
}