using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using LightDataAccess;

namespace Benchmarks;

internal class DbTest
{
    private static long BLToolkit_DB(int mappingsCount)
    {
        //DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

        var sw = new Stopwatch();
        sw.Start();

        //for (int i = 0; i < mappingsCount; i++)
        //{
        //    using (BLToolkit.Data.DbManager db = new BLToolkit.Data.DbManager())
        //    {
        //        List<Customer> list = db
        //            .SetCommand("SELECT * FROM Customers")
        //            .ExecuteList<Customer>();
        //    }
        //}

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long BLToolkit_DB2(int mappingsCount)
    {
        //BLToolkit.Data.DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

        var sw = new Stopwatch();
        sw.Start();

        //for (int i = 0; i < mappingsCount; i++)
        //{
        //    using (BLToolkit.Data.DbManager db = new BLToolkit.Data.DbManager())
        //    {
        //        List<test> list = db
        //            .SetCommand("SELECT * FROM test")
        //            .ExecuteList<test>();
        //    }
        //}

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long EmitMapper_DB(int mappingsCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        for (var i = 0; i < mappingsCount; i++)
            using (var con = new SqlConnection(
                       "Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
            using (var cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Customers";

                using (var reader = cmd.ExecuteReader())
                {
                    var list = reader.ToObjects<Customer>("reader1").ToList();
                }
            }

        sw.Stop();

        return sw.ElapsedMilliseconds;
    }

    private static long EmitMapper_DB2(int mappingsCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        for (var i = 0; i < mappingsCount; i++)
            using (var con = new SqlConnection(
                       "Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
            using (var cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM test";

                using (var reader = cmd.ExecuteReader())
                {
                    var list = reader.ToObjects<test>().ToList();
                }
            }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    public class test
    {
        public int col1;
        public int col2;
        public int col3;
        public int col4;
        public int? col5;
        public int? col6;
    }

    public class Customer
    {
        public string Address;
        public string City;
        public string CompanyName;
        public string ContactName;
        public string ContactTitle;
        public string Country;
        public string CustomerID;
        public string Fax;
        public string Phone;
        public string PostalCode;
        public string Region;
    }
}