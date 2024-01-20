namespace SamplesTests;

/// <summary>
///   Summary description for UnitTest1
/// </summary>
public class CustormTests
{
	/// <summary>
	///   Initializes a new instance of the <see cref="CustormTests" /> class.
	/// </summary>
	public CustormTests()
	{
		_connectionConfig = ConfigurationManager.ConnectionStrings["NorthWindSqlite"];
		_factory = DbProviderFactories.GetFactory(_connectionConfig.ProviderName);
	}

	private readonly ConnectionStringSettings _connectionConfig;

	private readonly DbProviderFactory _factory;

	/// <summary>
	///   Creates the connection.
	/// </summary>
	/// <returns>A DbConnection.</returns>
	private DbConnection CreateConnection()
	{
		var result = _factory.CreateConnection();
		result.ConnectionString = _connectionConfig.ConnectionString;
		result.Open();

		return result;
	}

	// [Fact]
	/// <summary>
	///   Gets the customers.
	/// </summary>
	public void GetCustomers()
	{
		Customer[] customers;

		using (var connection = CreateConnection())
		using (var cmd = _factory.CreateCommand())
		{
			cmd.Connection = connection;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "select * from [Customers]";
			using var reader = cmd.ExecuteReader();
			customers = reader.ToObjects<Customer>("reader1").ToArray();
		}

		customers.Length.ShouldBe(91);
	}

	// [Fact]
	/// <summary>
	///   Inserts the test.
	/// </summary>
	public void InsertTest()
	{
		using var ts = new TransactionScope();
		using var connection = CreateConnection();

		var rs = DbTools.InsertObject(
		  connection,
		  new
		  {
			  col1 = 10,
			  col2 = 11,
			  col3 = 12,
			  col4 = 13,
			  col5 = 1,
			  col6 = 2
		  },
		  "test",
		  DbSettings.Mssql).Result;

		ts.Complete();
		rs.ShouldBe(1);
	}

	// [Fact]
	/// <summary>
	///   Updates the customer.
	/// </summary>
	public void UpdateCustomer()
	{
		var objMan = new Mapper();

		var guid = Guid.NewGuid();

		// todo: there is a bug , In the callstack of DBTools and DataReaderToObjectMapper occur two times Reader.Read(); so..
		using var ts = new TransactionScope();
		using var connection = CreateConnection();

		var customer = DbTools.ExecuteReader(
		  connection,
		  "select * from Customers limit 1 ",
		  null,
		  r => r.ToObject<Customer>());

		customer.ShouldNotBeNull();

		var tracker = new ObjectsChangeTracker();
		tracker.RegisterObject(customer);
		customer.Address = guid.ToString();

		var result = DbTools.UpdateObject(
		  connection,
		  customer,
		  "Customers",
		  new[] { "CustomerID" },
		  tracker,
		  DbSettings.Mssql);

		result.Result.ShouldBe(1);
	}
}