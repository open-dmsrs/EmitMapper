using System;
using System.Collections.Generic;
using System.Text;

namespace EmitMapper.Tests;

using EmitMapper.Tests.TestData;

using Xunit;

public class DeepType
{
  [Fact]
  public void Test_DeepTypeCopy()
  {
    var customer = new Customer()
    {
      Address = new Address() { City = "istanbul", Country = "turkey", Id = 1, Street = "istiklal cad." },
      HomeAddress = new Address() { City = "istanbul", Country = "turkey", Id = 2, Street = "istiklal cad." },
      Id = 1,
      Name = "Eduardo Najera",
      Credit = 234.7m,
      WorkAddresses = new List<Address>()
                                          {
                                            new Address() {City = "istanbul", Country = "turkey", Id = 5, Street = "istiklal cad."},
                                            new Address() {City = "izmir", Country = "turkey", Id = 6, Street = "konak"}
                                          },
      Addresses = new[]
      {
        new Address() { City = "istanbul", Country = "turkey", Id = 3, Street = "istiklal cad." },
        new Address() { City = "izmir", Country = "turkey", Id = 4, Street = "konak" }
      }
    };
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Customer, CustomerDTO>();

    var result = mapper.Map(customer);

    Assert.Equal(customer.Name, result.Name);
    Assert.Equal(customer.Id, result.Id);
    Assert.Null(result.AddressCity);
    Equal(customer.Address, result.Address);

    for (int i = 0, n = result.Addresses.Length; i < n; i++)
    {
      Equal(customer.Addresses[i], result.Addresses[i]);
    }

    for (int i = 0, n = result.WorkAddresses.Count; i < n; i++)
    {
      Equal(customer.WorkAddresses[i], result.WorkAddresses[i]);
    }

  }

  private void Equal(Address a, Address ad)
  {
    Assert.Equal(a.Id, ad.Id);
    Assert.Equal(a.City, ad.City);
    Assert.Equal(a.Country, ad.Country);
    Assert.Equal(a.Street, ad.Street);
  }

  public void Equal(Address a, AddressDTO ad)
  {
    Assert.Equal(a.Id, ad.Id);
    Assert.Equal(a.City, ad.City);
    Assert.Equal(a.Country, ad.Country);
  }

}