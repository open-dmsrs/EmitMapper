using System;
using System.Collections.Generic;
using EmitMapper.Tests.TestData;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;

public class DeepType : IDisposable
{
  private readonly ITestOutputHelper _testOutputHelper;

  public DeepType(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  public void Dispose()
  {
  }

  [Fact]
  public void Test_DeepTypeCopy()
  {
    var customer = new Customer
    {
      Address =
        new Address { City = "istanbul", Country = "turkey", Id = 1, Street = "istiklal cad." },
      HomeAddress =
        new Address { City = "istanbul", Country = "turkey", Id = 2, Street = "istiklal cad." },
      Id = 1,
      Name = "Eduardo Najera",
      Credit = 234.7m,
      WorkAddresses =
        new List<Address>
        {
          new() { City = "istanbul", Country = "turkey", Id = 5, Street = "istiklal cad." },
          new() { City = "izmir", Country = "turkey", Id = 6, Street = "konak" }
        },
      Addresses = new[]
      {
        new Address { City = "istanbul", Country = "turkey", Id = 3, Street = "istiklal cad." },
        new Address { City = "izmir", Country = "turkey", Id = 4, Street = "konak" }
      }
    };

    var mapper = Mapper.Default.GetMapper<Customer, CustomerDTO>();

    var result = mapper.Map(customer);

    customer.Name.ShouldBe(result.Name);
    customer.Id.ShouldBe(result.Id);
    result.AddressCity.ShouldBeNull();
    Equal(customer.Address, result.Address);

    for (int i = 0, n = result.Addresses.Length; i < n; i++) Equal(customer.Addresses[i], result.Addresses[i]);

    for (int i = 0, n = result.WorkAddresses.Count; i < n; i++)
      Equal(customer.WorkAddresses[i], result.WorkAddresses[i]);
  }

  private void Equal(Address a, AddressDTO ad)
  {
    a.Id.ShouldBe(ad.Id);
    a.City.ShouldBe(ad.City);
    a.Country.ShouldBe(ad.Country);
  }

  public void Equal(Address a, Address ad)
  {
    a.Id.ShouldBe(ad.Id);
    a.City.ShouldBe(ad.City);
    a.Country.ShouldBe(ad.Country);
    a.Street.ShouldBe(ad.Street);
  }
}