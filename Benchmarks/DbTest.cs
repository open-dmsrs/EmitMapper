using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using LightDataAccess;


namespace Benchmarks
{
	class DbTest
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


		static long BLToolkit_DB(int mappingsCount)
		{
			BLToolkit.Data.DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var db = new BLToolkit.Data.DbManager())
				{
					var list = db
						.SetCommand("SELECT * FROM Customers")
						.ExecuteList<Customer>();
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BLToolkit_DB2(int mappingsCount)
		{
			BLToolkit.Data.DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var db = new BLToolkit.Data.DbManager())
				{
					var list = db
						.SetCommand("SELECT * FROM test")
						.ExecuteList<test>();
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long EmitMapper_DB(int mappingsCount)
		{
			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
				using (var cmd = con.CreateCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = "SELECT * FROM Customers";

					using (var reader = cmd.ExecuteReader())
					{
						var list = reader.ToObjects<Customer>("reader1").ToList();
					}
				}
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}


		static long EmitMapper_DB2(int mappingsCount)
		{
			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
				using (var cmd = con.CreateCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = "SELECT * FROM test";

					using (var reader = cmd.ExecuteReader())
					{
						var list = reader.ToObjects<test>().ToList();
					}
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}
	}
}
