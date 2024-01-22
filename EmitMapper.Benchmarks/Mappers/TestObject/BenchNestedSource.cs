namespace EmitMapper.Benchmarks.Mappers.TestObject;

/// <summary>
///   The bench nested source.
/// </summary>
public class BenchNestedSource : ITestObject
{
	/// <summary>
	/// I1
	/// </summary>
	public Nested1? I1;

	/// <summary>
	/// I2
	/// </summary>
	public Nested1? I2;

	/// <summary>
	/// I3
	/// </summary>
	public Nested1? I3;

	/// <summary>
	/// I4
	/// </summary>
	public Nested1? I4;

	/// <summary>
	/// I5
	/// </summary>
	public Nested1? I5;

	/// <summary>
	/// I6
	/// </summary>
	public Nested1? I6;

	/// <summary>
	/// I7
	/// </summary>
	public Nested1? I7;

	/// <summary>
	/// I8
	/// </summary>
	public Nested1? I8;

	/// <summary>
	/// N2
	/// </summary>
	public int N2;

	/// <summary>
	/// N3
	/// </summary>
	public long N3;

	/// <summary>
	/// N4
	/// </summary>
	public byte N4;

	/// <summary>
	/// N5
	/// </summary>
	public short N5;

	/// <summary>
	/// N6
	/// </summary>
	public uint N6;

	/// <summary>
	/// N7
	/// </summary>
	public int N7;

	/// <summary>
	/// N8
	/// </summary>
	public int N8;

	/// <summary>
	/// N9
	/// </summary>
	public int N9;

	/// <summary>
	/// S1
	/// </summary>
	public string? S1;

	/// <summary>
	/// S2
	/// </summary>
	public string? S2;

	/// <summary>
	/// S3
	/// </summary>
	public string? S3;

	/// <summary>
	/// S4
	/// </summary>
	public string? S4;

	/// <summary>
	/// S5
	/// </summary>
	public string? S5;

	/// <summary>
	/// S6
	/// </summary>
	public string? S6;

	/// <summary>
	/// S7
	/// </summary>
	public string? S7;

	/// <summary>
	///   The nested1.
	/// </summary>
	public class Nested1
	{
		/// <summary>
		/// I1
		/// </summary>
		public Nested2? I1;

		/// <summary>
		/// I2
		/// </summary>
		public Nested2? I2;

		/// <summary>
		/// I3
		/// </summary>
		public Nested2? I3;

		/// <summary>
		/// I4
		/// </summary>
		public Nested2? I4;

		/// <summary>
		/// I5
		/// </summary>
		public Nested2? I5;

		/// <summary>
		/// I6
		/// </summary>
		public Nested2? I6;

		/// <summary>
		/// I7
		/// </summary>
		public Nested2? I7;
	}

	/// <summary>
	///   The nested2.
	/// </summary>
	public class Nested2
	{
		/// <summary>
		/// I
		/// </summary>
		public int I;

		/// <summary>
		/// Str1
		/// </summary>
		public string? Str1;

		/// <summary>
		/// Str2
		/// </summary>
		public string? Str2;
	}
}