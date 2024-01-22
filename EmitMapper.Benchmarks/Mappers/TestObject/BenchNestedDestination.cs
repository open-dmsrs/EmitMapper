namespace EmitMapper.Benchmarks.Mappers.TestObject;

/// <summary>
///   The bench nested destination.
/// </summary>
public class BenchNestedDestination : ITestObject
{
	/// <summary>
	/// N2
	/// </summary>
	public long N2;

	/// <summary>
	/// N3
	/// </summary>
	public long N3;

	/// <summary>
	/// N4
	/// </summary>
	public long N4;

	/// <summary>
	/// N5
	/// </summary>
	public long N5;

	/// <summary>
	/// N6
	/// </summary>
	public long N6;

	/// <summary>
	/// N7
	/// </summary>
	public long N7;

	/// <summary>
	/// N8
	/// </summary>
	public long N8;

	/// <summary>
	/// N9
	/// </summary>
	public long N9;

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
	///   Gets or Sets the i1.
	/// </summary>
	public Inner1? I1 { get; set; }

	/// <summary>
	///   Gets or Sets the i2.
	/// </summary>
	public Inner1? I2 { get; set; }

	/// <summary>
	///   Gets or Sets the i3.
	/// </summary>
	public Inner1? I3 { get; set; }

	/// <summary>
	///   Gets or Sets the i4.
	/// </summary>
	public Inner1? I4 { get; set; }

	/// <summary>
	///   Gets or Sets the i5.
	/// </summary>
	public Inner1? I5 { get; set; }

	/// <summary>
	///   Gets or Sets the i6.
	/// </summary>
	public Inner1? I6 { get; set; }

	/// <summary>
	///   Gets or Sets the i7.
	/// </summary>
	public Inner1? I7 { get; set; }

	/// <summary>
	///   Gets or Sets the i8.
	/// </summary>
	public Inner1? I8 { get; set; }

	/// <summary>
	///   The inner1.
	/// </summary>
	public class Inner1
	{
		/// <summary>
		/// I1
		/// </summary>
		public Inner2? I1;

		/// <summary>
		/// I2
		/// </summary>
		public Inner2? I2;

		/// <summary>
		/// I3
		/// </summary>
		public Inner2? I3;

		/// <summary>
		/// I4
		/// </summary>
		public Inner2? I4;

		/// <summary>
		/// I5
		/// </summary>
		public Inner2? I5;

		/// <summary>
		/// I6
		/// </summary>
		public Inner2? I6;

		/// <summary>
		/// I7
		/// </summary>
		public Inner2? I7;
	}

	/// <summary>
	///   The inner2.
	/// </summary>
	public class Inner2
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