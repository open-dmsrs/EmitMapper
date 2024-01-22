namespace EmitMapper.Tests;

/// <summary>
/// The relative path.
/// </summary>
public class RelativePath
{
	private readonly ITestOutputHelper _outputHelper;

	/// <summary>
	/// Initializes a new instance of the <see cref="RelativePath"/> class.
	/// </summary>
	/// <param name="outputHelper">The output helper.</param>
	public RelativePath(ITestOutputHelper outputHelper)
	{
		Console.WriteLine(outputHelper.GetType().FullName);

		this._outputHelper = outputHelper;
	}

	/// <summary>
	/// Gets the relative path.
	/// </summary>
	[Fact]
	public void GetRelativePath()
	{
		var parent = Directory.GetCurrentDirectory();
		var sub = Path.Combine(parent, @"Users/Default");

		_outputHelper.WriteLine(sub);
		_outputHelper.WriteLine(parent);
		var result = Path.GetRelativePath(parent, sub);
		result.ShouldBe(Path.Combine("Users", "Default"));
		result = Path.GetRelativePath(sub, parent);
		result.ShouldBe(@".." + Path.DirectorySeparatorChar + "..");
	}
}
