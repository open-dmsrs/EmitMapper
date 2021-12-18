using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Transactions;
using LightDataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmitMapper.Samples.SamplesTests;

public class Customer
{
    public string Address;
    public string City;
    public string CompanyName;
    public string ContactName;
    public string ContactTitle;
    public string Country;
    public string CustomerId;
    public string Fax;
    public string Phone;
    public string PostalCode;
    public string Region;
}

/// <summary>
///     Summary description for UnitTest1
/// </summary>
[TestClass]
public class CustormTests
{
    private readonly ConnectionStringSettings _connectionConfig;

    private readonly DbProviderFactory _factory;

    public CustormTests()
    {
        _connectionConfig = ConfigurationManager.ConnectionStrings["NorthWindSqlite"];
        _factory = DbProviderFactories.GetFactory(_connectionConfig.ProviderName);
    }

    private DbConnection CreateConnection()
    {
        var result = _factory.CreateConnection();
        result.ConnectionString = _connectionConfig.ConnectionString;
        result.Open();
        return result;
    }

    [TestMethod]
    public void GetCustomers()
    {
        Customer[] customers;
        using (var connection = CreateConnection())
        using (var cmd = _factory.CreateCommand())
        {
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [Customers]";
            using (var reader = cmd.ExecuteReader())
            {
                customers = reader.ToObjects<Customer>("reader1").ToArray();
            }
        }

        Assert.IsTrue(customers.Length == 91);
    }

    [TestMethod]
    public void UpdateCustomer()
    {
        var objMan = new ObjectMapperManager();

        var guid = Guid.NewGuid();
        // todo: there is a bug , In the callstack of DBTools and DataReaderToObjectMapper ocur two times Reader.Read(); so..

        using (var ts = new TransactionScope())
        using (var connection = CreateConnection())
        {
            var customer = DbTools.ExecuteReader(
                connection,
                "select * from Customers limit 1 ",
                null,
                r => r.ToObject<Customer>()
            );
            Assert.IsNotNull(customer);


            var tracker = new ObjectsChangeTracker();
            tracker.RegisterObject(customer);
            customer.Address = guid.ToString();

            var result = DbTools.UpdateObject(
                connection,
                customer,
                "Customers",
                new[] { "CustomerID" },
                tracker,
                DbSettings.Mssql
            );
            Assert.IsTrue(result.Result == 1);
        }
    }

    [TestMethod]
    public void InsertTest()
    {
        using (var ts = new TransactionScope())
        using (var connection = CreateConnection())
        {
            var rs = DbTools.InsertObject(
                connection,
                new { col1 = 10, col2 = 11, col3 = 12, col4 = 13, col5 = 1, col6 = 2 },
                "test",
                DbSettings.Mssql).Result;
            ts.Complete();
            Assert.IsTrue(rs == 1);
        }
    }
}