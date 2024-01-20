namespace EmitMapper.Tests;

/// <summary>
///   The nullable types.
/// </summary>
public class NullableTypes
{
	/// <summary>
	///   Nullable_to_s the value.
	/// </summary>
	[Fact]
	public void NullableToValue()
	{
		var mapper = Mapper.Default.GetMapper<B1, A1>(
		  new DefaultMapConfig().NullSubstitution<B1.Int1, A1.Int1>(state => new A1.Int1(0))
			.NullSubstitution<int?, int>(state => 3).NullSubstitution<int?, int?>(state => 4));

		// DynamicAssemblyManager.SaveAssembly();
		var a = mapper.Map(new B1());
		a.Fld1.ShouldBe(10);
		a.I.ShouldNotBeNull();
		a.I.S.ShouldBe("A1::Int1::s");
		a.Fld2.ShouldBe(3);
		a.Fld3.ShouldBe(4);
	}

	/// <summary>
	///   Nullables the struct_to_ struct.
	/// </summary>
	[Fact]
	public void NullableStructToStruct()
	{
		var bint = new B3.BInt { Fld1 = "b" };

		var b = new B3
		{
			Fld1 = bint,
			Fld2 = bint,
			Fld3 = bint,
			Fld4 = bint,
			Fld6 = bint,
			Fld7 = bint
		};

		var mapper = Mapper.Default.GetMapper<B3, A3>();

		// DynamicAssemblyManager.SaveAssembly();
		var a = mapper.Map(b);
		a.Fld1.Value.Fld1.ShouldBe("b");
		a.Fld2.Value.Fld1.ShouldBe("b");
		a.Fld3.Value.Fld1.ShouldBe("b");
		a.Fld4.Value.Fld1.ShouldBe("b");
		a.Fld6.Value.Fld1.ShouldBe("b");
		a.Fld7.Fld1.ShouldBe("b");
		a.Fld2.Value.Fld3.ShouldBe("a");
		a.Fld5.HasValue.ShouldBeFalse();
	}

	/// <summary>
	///   Struct_to_s the nullable struct.
	/// </summary>
	[Fact]
	public void StructToNullableStruct()
	{
		var bint = new B4.BInt { Fld1 = "b" };
		var b = new B4 { Fld1 = bint };
		var a = Context.ObjMan.GetMapper<B4, A4>().Map(b);
		a.Fld1.Fld1.ShouldBe("b");
	}

	/// <summary>
	///   Test_s the nullable.
	/// </summary>
	[Fact]
	public void TestNullable()
	{
		var a = Context.ObjMan.GetMapper<B5, A5>().Map(new B5());
		a.Fld1.Value.ShouldBe(10);
		a.Fld2.ShouldBeNull();
		A5.En.Value2.ShouldBe(a.Fld3.Value);
		A5.En.Value3.ShouldBe(a.Fld4);
		a.Fld5.Value.ShouldBe(13);
		a.Fld6.Value.ShouldBe(11);
	}

	/// <summary>
	///   Test_s the object_ nullable.
	/// </summary>
	[Fact]
	public void TestObjectNullable()
	{
		var a = Mapper.Default
		  .GetMapper<B6, A6>(new DefaultMapConfig().DeepMap().ConvertUsing<object, object>(v => null)).Map(new B6());

		a.ShouldBeNull();
	}

	/// <summary>
	///   Test_s the object_ nullable7.
	/// </summary>
	[Fact]
	public void TestObjectNullable7()
	{
		var a = Mapper.Default
		  .GetMapper<B7, A7>(new DefaultMapConfig().DeepMap().ConvertUsing<object, int>(v => 100)).Map(new B7());

		a.I.ShouldBe(100);
	}

	/// <summary>
	///   Value_to_s the nullable.
	/// </summary>
	[Fact]
	public void ValueToNullable()
	{
		var a = Context.ObjMan.GetMapper<B2, A2>().Map(new B2());
		a.Fld1.ShouldBe(10);
	}

	/// <summary>
	///   The a1.
	/// </summary>
	public class A1
	{
		public int Fld1;
		public int Fld2;
		public int? Fld3;
		public Int1? I;

		/// <summary>
		///   The int1.
		/// </summary>
		public class Int1
		{
			public string S;

			/// <summary>
			///   Initializes a new instance of the <see cref="Int1" /> class.
			/// </summary>
			public Int1()
			{
			}

			/// <summary>
			///   Initializes a new instance of the <see cref="Int1" /> class.
			/// </summary>
			/// <param name="i">The i.</param>
			public Int1(int i)
			{
				S = "A1::Int1::s";
			}
		}
	}

	/// <summary>
	///   The a2.
	/// </summary>
	public class A2
	{
		public int? Fld1;
	}

	/// <summary>
	///   The a3.
	/// </summary>
	public class A3
	{
		public AInt? Fld1;
		public AInt? Fld4;
		public AInt? Fld5;
		public AInt? Fld6;
		public AInt Fld7;

		/// <summary>
		///   Initializes a new instance of the <see cref="A3" /> class.
		/// </summary>
		public A3()
		{
			var a = new AInt { Fld3 = "a" };
			Fld2 = a;
			Fld5 = a;
		}

		/// <summary>
		///   Gets or Sets the fld2.
		/// </summary>
		public AInt? Fld2 { get; set; }
		/// <summary>
		///   Gets or Sets the fld3.
		/// </summary>
		public AInt? Fld3 { get; set; }

		public struct AInt
		{
			public string Fld1;
			public string Fld2;
			public string Fld3;
		}
	}

	/// <summary>
	///   The a4.
	/// </summary>
	public class A4
	{
		public AInt Fld1;

		public struct AInt
		{
			public string Fld1;
		}
	}

	/// <summary>
	///   The a5.
	/// </summary>
	public class A5
	{
		public int? Fld1 = 0;
		public int? Fld2 = 10;
		public En? Fld3 = En.Value1;
		public En Fld4 = En.Value1;
		public int? Fld5 = 0;
		public int? Fld6 = null;

		/// <summary>
		///   The en.
		/// </summary>
		public enum En
		{
			Value1,
			Value2,
			Value3
		}
	}

	/// <summary>
	///   The a6.
	/// </summary>
	public class A6
	{
		/// <summary>
		///   Gets or Sets the dt.
		/// </summary>
		public DateTime? Dt { get; set; }
		/// <summary>
		///   Gets or Sets the i.
		/// </summary>
		public int? I { get; set; }
	}

	/// <summary>
	///   The a7.
	/// </summary>
	public class A7
	{
		/// <summary>
		///   Gets or Sets the i.
		/// </summary>
		public int? I { get; set; }
	}

	/// <summary>
	///   The b1.
	/// </summary>
	public class B1
	{
		public int? Fld1 = 10;
		public int? Fld2;
		public int? Fld3;
		public Int1? I;

		/// <summary>
		///   The int1.
		/// </summary>
		public class Int1
		{
			public string S = "B1::Int1::s";
		}
	}

	/// <summary>
	///   The b2.
	/// </summary>
	public class B2
	{
		public int Fld1 = 10;
	}

	/// <summary>
	///   The b3.
	/// </summary>
	public class B3
	{
		public BInt? Fld1;
		public BInt? Fld3;
		public BInt? Fld5;
		public BInt Fld6;
		public BInt? Fld7;
		/// <summary>
		///   Gets or Sets the fld2.
		/// </summary>
		public BInt? Fld2 { get; set; }
		/// <summary>
		///   Gets or Sets the fld4.
		/// </summary>
		public BInt? Fld4 { get; set; }

		public struct BInt
		{
			public string Fld1;
			public string Fld2;
		}
	}

	/// <summary>
	///   The b4.
	/// </summary>
	public class B4
	{
		public BInt? Fld1 = new BInt();

		public struct BInt
		{
			public string Fld1;
		}
	}

	/// <summary>
	///   The b5.
	/// </summary>
	public class B5
	{
		public int Fld1 = 10;
		public string Fld2 = null;
		public En Fld3 = En.Value2;
		public En? Fld4 = En.Value3;
		public int? Fld5 = 13;
		public string Fld6 = "11";

		/// <summary>
		///   The en.
		/// </summary>
		public enum En
		{
			Value1,
			Value2,
			Value3
		}
	}

	/// <summary>
	///   The b6.
	/// </summary>
	public class B6;

	/// <summary>
	///   The b7.
	/// </summary>
	public class B7
	{
		public decimal I = 100;
	}
}