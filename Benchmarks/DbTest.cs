using LightDataAccess;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;


namespace Benchmarks
{
    internal class DbTest
    {
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

        private static long BLToolkit_DB(int mappingsCount)
        {
            //DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

            Stopwatch sw = new Stopwatch();
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

            Stopwatch sw = new Stopwatch();
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < mappingsCount; i++)
            {
                using (SqlConnection con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT * FROM Customers";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Customer> list = reader.ToObjects<Customer>("reader1").ToList();
                    }
                }
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long EmitMapper_DB2(int mappingsCount)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < mappingsCount; i++)
            {
                using (SqlConnection con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT * FROM test";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<test> list = reader.ToObjects<test>().ToList();
                    }
                }
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}
