using System;
using System.Collections.Generic;
using EmitMapper.Tests.TestData;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;
/// <summary>
/// The deep type.
/// </summary>

public class DeepType : IDisposable
{
  private readonly ITestOutputHelper _testOutputHelper;

  /// <summary>
  /// Initializes a new instance of the <see cref="DeepType"/> class.
  /// </summary>
  /// <param name="testOutputHelper">The test output helper.</param>
  public DeepType(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  /// <summary>
  /// 
  /// </summary>
  public void Dispose()
  {
  }

  /// <summary>
  /// Test_s the deep type copy.
  /// </summary>
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
    IsSame(customer.Address, result.Address);

    for (int i = 0, n = result.Addresses.Length; i < n; i++) IsSame(customer.Addresses[i], result.Addresses[i]);

    for (int i = 0, n = result.WorkAddresses.Count; i < n; i++)
      IsSame(customer.WorkAddresses[i], result.WorkAddresses[i]);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="a">The a.</param>
  /// <param name="ad">The ad.</param>
  private static void IsSame(Address a, AddressDTO ad)
  {
    a.Id.ShouldBe(ad.Id);
    a.City.ShouldBe(ad.City);
    a.Country.ShouldBe(ad.Country);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="a">The a.</param>
  /// <param name="ad">The ad.</param>

  private static void IsSame(Address a, Address ad)
  {
    a.Id.ShouldBe(ad.Id);
    a.City.ShouldBe(ad.City);
    a.Country.ShouldBe(ad.Country);
    a.Street.ShouldBe(ad.Street);
  }
}