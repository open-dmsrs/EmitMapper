namespace EmitMapper.Benchmarks.Mappers.TestObject;

/// <summary>
///   The bench nested source.
/// </summary>
public class BenchNestedSource : ITestObject
{
	public Nested1? I1;

	public Nested1? I2;

	public Nested1? I3;

	public Nested1? I4;

	public Nested1? I5;

	public Nested1? I6;

	public Nested1? I7;

	public Nested1? I8;

	public int N2;

	public long N3;

	public byte N4;

	public short N5;

	public uint N6;

	public int N7;

	public int N8;

	public int N9;

	public string? S1;

	public string? S2;

	public string? S3;

	public string? S4;

	public string? S5;

	public string? S6;

	public string? S7;

	/// <summary>
	///   The nested1.
	/// </summary>
	public class Nested1
	{
		public Nested2? I1;

		public Nested2? I2;

		public Nested2? I3;

		public Nested2? I4;

		public Nested2? I5;

		public Nested2? I6;

		public Nested2? I7;
	}

	/// <summary>
	///   The nested2.
	/// </summary>
	public class Nested2
	{
		public int I;

		public string? Str1;

		public string? Str2;
	}
}