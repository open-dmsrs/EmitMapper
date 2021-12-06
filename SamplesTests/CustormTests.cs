using EmitMapper;
using LightDataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Transactions;

namespace SamplesTests
{
    public class Customer
    {
        public string CustomerID;
        public string CompanyName;
        public string ContactName;
        public string ContactTitle;
        public string Address;
        public string City;
        public string Region;
        public string PostalCode;
        public string Country;
        public string Phone;
        public string Fax;
    }

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CustormTests
    {
        public CustormTests()
        {
            _connectionConfig = ConfigurationManager.ConnectionStrings["NorthWindSqlite"];
            _factory = DbProviderFactories.GetFactory(_connectionConfig.ProviderName);
        }

        private readonly DbProviderFactory _factory;
        private readonly ConnectionStringSettings _connectionConfig;

        private DbConnection CreateConnection()
        {
            DbConnection result = _factory.CreateConnection();
            result.ConnectionString = _connectionConfig.ConnectionString;
            result.Open();
            return result;
        }

        [TestMethod]
        public void GetCustomers()
        {
            Customer[] customers;
            using (DbConnection connection = CreateConnection())
            using (DbCommand cmd = _factory.CreateCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select * from [Customers]";
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    customers = reader.ToObjects<Customer>("reader1").ToArray();
                }
            }

            Assert.IsTrue(customers.Length == 91);
        }

        [TestMethod]
        public void UpdateCustomer()
        {
            ObjectMapperManager objMan = new ObjectMapperManager();

            Guid guid = Guid.NewGuid();
            // todo: there is a bug , In the callstack of DBTools and DataReaderToObjectMapper ocur two times Reader.Read(); so..

            using (TransactionScope ts = new TransactionScope())
            using (DbConnection connection = CreateConnection())
            {
                Customer customer = DBTools.ExecuteReader(
                    connection,
                    "select * from Customers limit 1 ",
                    null,
                    r => r.ToObject<Customer>()
                );
                Assert.IsNotNull(customer);



                ObjectsChangeTracker tracker = new ObjectsChangeTracker();
                tracker.RegisterObject(customer);
                customer.Address = guid.ToString();

                System.Threading.Tasks.Task<int> result = DBTools.UpdateObject(
                          connection,
                          customer,
                          "Customers",
                          new[] { "CustomerID" },
                          tracker,
                          DbSettings.MSSQL
                      );
                Assert.IsTrue(result.Result == 1);
            }
        }
        [TestMethod]
        public void InsertTest()
        {
            using (TransactionScope ts = new TransactionScope())
            using (DbConnection connection = CreateConnection())
            {
                int rs = DBTools.InsertObject(connection, new { col1 = 10, col2 = 11, col3 = 12, col4 = 13, col5 = 1, col6 = 2 }, "test", DbSettings.MSSQL).Result;
                ts.Complete();
                Assert.IsTrue(rs == 1);
            }
        }
    }
}
