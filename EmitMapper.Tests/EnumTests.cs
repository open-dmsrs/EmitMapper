namespace EmitMapper.Tests;

/// <summary>
///   The enum tests.
/// </summary>
public class EnumTests
{
	/// <summary>
	///   Enums the tests1.
	/// </summary>
	[Fact]
	public void EnumTests1()
	{
		var mapper = Context.ObjMan.GetMapper<B, A>();

		// DynamicAssemblyManager.SaveAssembly();
		var a = mapper.Map(new B());

		a.En1.ShouldBe(En1.C);
		a.En2.ShouldBe(En2.C);
		a.En3.ShouldBe(En3.C);
		a.En4.ShouldBe(2);
		a.En6.ShouldBe(En1.C);
		a.En7.ShouldBe(En3.C);
		a.En8.ShouldBe(En3.C);
		a.En9.ShouldBeNull();
	}

	/// <summary>
	///   The a.
	/// </summary>
	public class A
	{
		/// <summary>
		/// En2
		/// </summary>
		public En2 En2;

		/// <summary>
		/// En4
		/// </summary>
		public decimal En4;

		/// <summary>
		/// En5
		/// </summary>
		public string? En5;

		/// <summary>
		/// En6
		/// </summary>
		public En1? En6;

		/// <summary>
		/// En7
		/// </summary>
		public En3 En7;

		/// <summary>
		/// En8
		/// </summary>
		public En3? En8;

		/// <summary>
		/// En9
		/// </summary>
		public En3? En9 = En3.C;

		/// <summary>
		///   Gets or Sets the en1.
		/// </summary>
		public En1 En1 { get; set; }

		/// <summary>
		///   Gets or Sets the en3.
		/// </summary>
		public En3 En3 { get; set; }
	}

	/// <summary>
	///   The b.
	/// </summary>
	public class B
	{
		/// <summary>
		/// En1
		/// </summary>
		public decimal En1 = 3;

		/// <summary>
		/// En3
		/// </summary>
		public string En3 = "C";

		/// <summary>
		/// En4
		/// </summary>
		public En2 En4 = EnumTests.En2.B;

		/// <summary>
		/// En5
		/// </summary>
		public En3 En5 = EnumTests.En3.A;

		/// <summary>
		/// En6
		/// </summary>
		public En2 En6 = EnumTests.En2.C;

		/// <summary>
		/// En7
		/// </summary>
		public En1? En7 = EnumTests.En1.C;

		/// <summary>
		/// En8
		/// </summary>
		public En1? En8 = EnumTests.En1.C;

		/// <summary>
		/// En9
		/// </summary>
		public En2? En9 = null;

		/// <summary>
		///   Initializes a new instance of the <see cref="B" /> class.
		/// </summary>
		public B()
		{
			En2 = EnumTests.En1.C;
		}

		/// <summary>
		///   Gets or Sets the en2.
		/// </summary>
		public En1 En2 { get; set; }
	}

	/// <summary>
	///   The en1.
	/// </summary>
	public enum En1 : byte
	{
		/// <summary>
		/// A
		/// </summary>
		A = 1,

		/// <summary>
		/// B
		/// </summary>
		B = 2,

		/// <summary>
		/// C
		/// </summary>
		C = 3
	}

	/// <summary>
	///   The en2.
	/// </summary>
	public enum En2 : long
	{
		/// <summary>
		/// A
		/// </summary>
		A = 1,

		/// <summary>
		/// B
		/// </summary>
		B = 2,

		/// <summary>
		/// C
		/// </summary>
		C = 3
	}

	/// <summary>
	///   The en3.
	/// </summary>
	public enum En3
	{
		/// <summary>
		/// B
		/// </summary>
		B = 2,

		/// <summary>
		/// C
		/// </summary>
		C = 3,

		/// <summary>
		/// A
		/// </summary>
		A = 1
	}
}