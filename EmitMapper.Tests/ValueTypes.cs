namespace EmitMapper.Tests;

/// <summary>
///   The value types.
/// </summary>
public class ValueTypes
{
	/// <summary>
	///   Test_s the class to struct.
	/// </summary>
	[Fact]
	public void TestClassToStruct()
	{
		var a = new A1();
		var b = new B1();
		a = Context.ObjMan.GetMapper<B1, A1>().Map(b, a);
		a.Fld1.ShouldBe(10);
	}

	/// <summary>
	///   Test_s the nested structs.
	/// </summary>
	[Fact]
	public void TestNestedStructs()
	{
		var mapper = Mapper.Default.GetMapper<B6From, A6To>();

		// DynamicAssemblyManager.SaveAssembly();
		var b = new B6From();

		var bs2 = new B6From.S2Struct();
		bs2.S.I = 15;
		b.S2 = bs2;
		b.S.S.I = 13;
		b.S3.S.I = 10;
		b.S4 = new B6From.C1();
		b.S4.S.I = 11;
		b.S5 = new B6From.C3Class { C1 = new B6From.C1() };
		b.S5.C1.S.I = 1;
		b.S5.C2 = new B6From.C1();
		b.S5.C2.S.I = 2;
		b.S5.C3 = new B6From.C1();
		b.S5.C3.S.I = 3;

		var a = mapper.Map(b);
		a.S.S.I.ShouldBe(13);
		a.S2.S.I.ShouldBe(15);
		a.S3.S.I.ShouldBe(10);
		a.S4.S.I.ShouldBe(11);
		a.S5.C1.S.I.ShouldBe(1);
		a.S5.C2.S.I.ShouldBe(2);
		a.S5.C3.S.I.ShouldBe(3);
	}

	/// <summary>
	///   Test_s the struct fields.
	/// </summary>
	[Fact]
	public void TestStructFields()
	{
		var mapper = Mapper.Default.GetMapper<B5, A5>();

		// DynamicAssemblyManager.SaveAssembly();
		var b = new B5();
		b.A.Fld1 = 10;
		var a = mapper.Map(b);
		a.A.Fld1.ShouldBe(10);
	}

	/// <summary>
	///   Test_s the struct properties.
	/// </summary>
	[Fact]
	public void TestStructProperties()
	{
		var a = new A4();
		var b = new B4();
		var mapper = Context.ObjMan.GetMapper<B4, A4>();

		// DynamicAssemblyManager.SaveAssembly();
		a = mapper.Map(b, a);
		b.Fld1.Fld1.ToString().ShouldBe(a.Fld1.Fld1);
		b.Fld2.Fld1.ToString().ShouldBe(a.Fld2.Fld1);
		b.Fld3.Fld1.ToString().ShouldBe(a.Fld3.Fld1);
	}

	/// <summary>
	///   Test_s the struct to class.
	/// </summary>
	[Fact]
	public void TestStructToClass()
	{
		var a = new A3();
		var b = new B3 { Fld1 = 87 };
		a = Context.ObjMan.GetMapper<B3, A3>().Map(b, a);
		a.Fld1.ShouldBe(87);
	}

	/// <summary>
	///   Test_s the struct to struct.
	/// </summary>
	[Fact]
	public void TestStructToStruct()
	{
		var a = new A2();
		var b = new B2 { Fld1 = 99 };
		a = Context.ObjMan.GetMapper<B2, A2>().Map(b, a);
		a.Fld1.ShouldBe(99);
	}

	/// <summary>
	///   The a3.
	/// </summary>
	public class A3
	{
		public int Fld1;
	}

	/// <summary>
	///   The a5.
	/// </summary>
	public class A5
	{
		public A1 A;
	}

	/// <summary>
	///   The a6 to.
	/// </summary>
	public class A6To
	{
		public S2Struct S2;
		public C2? S4;
		public C3Class? S5;
		/// <summary>
		///   Gets or Sets the s.
		/// </summary>
		public S2Struct S { get; set; }
		/// <summary>
		///   Gets or Sets the s3.
		/// </summary>
		public C1? S3 { get; set; }

		public struct S1
		{
			public int I { get; set; }
		}

		public struct S2Struct
		{
			public S1 S { get; set; }
		}

		/// <summary>
		///   The c1.
		/// </summary>
		public class C1
		{
			/// <summary>
			///   Gets or Sets the s.
			/// </summary>
			public S1 S { get; set; }
		}

		/// <summary>
		///   The c2.
		/// </summary>
		public class C2
		{
			public S1 S;
		}

		/// <summary>
		///   The c3 class.
		/// </summary>
		public class C3Class
		{
			public C2? C1;
			public C2? C2;
			public C2? C3;
		}
	}

	/// <summary>
	///   The b1.
	/// </summary>
	public class B1
	{
		public int Fld1 = 10;
	}

	/// <summary>
	///   The b2.
	/// </summary>
	public class B2
	{
		public int Fld1;
	}

	/// <summary>
	///   The b4.
	/// </summary>
	public class B4
	{
		public Int Fld3;

		/// <summary>
		///   Initializes a new instance of the <see cref="B4" /> class.
		/// </summary>
		public B4()
		{
			Fld1 = new Int { Fld1 = 12.444M };
			Fld2 = new Int { Fld1 = 1111 };
			Fld3.Fld1 = 444;
		}

		/// <summary>
		///   Gets or Sets the fld1.
		/// </summary>
		public Int Fld1 { get; set; }
		/// <summary>
		///   Gets or Sets the fld2.
		/// </summary>
		public Int Fld2 { get; set; }

		public struct Int
		{
			public decimal Fld1;
		}
	}

	/// <summary>
	///   The b5.
	/// </summary>
	public class B5
	{
		public A1 A;
	}

	/// <summary>
	///   The b6 from.
	/// </summary>
	public class B6From
	{
		public S2Struct S = new();
		public S2Struct S3;
		public C3Class? S5;
		/// <summary>
		///   Gets or Sets the s2.
		/// </summary>
		public S2Struct S2 { get; set; }
		/// <summary>
		///   Gets or Sets the s4.
		/// </summary>
		public C1? S4 { get; set; }

		public struct S1
		{
			public int I { get; set; }
		}

		public struct S2Struct
		{
			public S1 S;
		}

		/// <summary>
		///   The c1.
		/// </summary>
		public class C1
		{
			public S1 S;
		}

		/// <summary>
		///   The c3 class.
		/// </summary>
		public class C3Class
		{
			public C1? C1;
			public C1? C2;
			public C1? C3;
		}
	}

	public struct A1
	{
		public int Fld1;
	}

	public struct A2
	{
		public int Fld1;
	}

	public struct A4
	{
		public Int Fld2;
		public Int Fld1 { get; set; }
		public Int Fld3 { get; set; }

		public struct Int
		{
			public string Fld1;
		}
	}

	public struct B3
	{
		public int Fld1;
	}
}