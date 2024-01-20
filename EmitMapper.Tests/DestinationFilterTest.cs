namespace EmitMapper.Tests;

/// <summary>
///   The destination filter test.
/// </summary>
public class DestinationFilterTest
{
	/// <summary>
	///   Test_s the derived.
	/// </summary>
	[Fact]
	public void Test_Derived()
	{
		var mapper = Mapper.Default.GetMapper<IDerived, Target>();

		var source = new Derived { BaseProperty = "base", DerivedProperty = "derived" };

		var destination = mapper.Map(source);
		destination.BaseProperty.ShouldBe("base");
		destination.DerivedProperty.ShouldBe("derived");
	}

	/// <summary>
	///   Testdestinations the filter.
	/// </summary>
	[Fact]
	public void TestdestinationFilter()
	{
		var mapper = Mapper.Default.GetMapper<DestinationTestFilterSrc, DestinationTestFilterDest>(
		  new DefaultMapConfig().FilterDestination<string>((value, state) => false)
			.FilterDestination<int>((value, state) => value >= 0).FilterSource<int>((value, state) => value >= 10)
			.FilterSource<object>(
			  (value, state) => value is not long
								&& (value is not DestinationTestFilterSrc
									|| (value as DestinationTestFilterSrc).I1 != 666)));

		var dest = mapper.Map(new DestinationTestFilterSrc());

		dest.I1.ShouldBe(13);
		dest.I2.ShouldBe(-5);
		dest.I3.ShouldBe(0);
		dest.L1.ShouldBe(0);
		dest.Str.ShouldBeNull();

		dest = mapper.Map(new DestinationTestFilterSrc { I1 = 666 }, new DestinationTestFilterDest());
		dest.I1.ShouldBe(0);
	}

	/// <summary>
	///   Testdestinations the filter1.
	/// </summary>
	[Fact]
	public void TestdestinationFilter1()
	{
		var mapper = Mapper.Default.GetMapper<DestinationTestFilterSrc, DestinationTestFilterDest>(
		  new DefaultMapConfig().FilterDestination<string>((value, state) => false)
			.FilterDestination<int>((value, state) => value >= 0).FilterSource<int>((value, state) => value >= 10)
			.FilterSource<object>(
			  (value, state) => value is not long
								&& (value is not DestinationTestFilterSrc
									|| (value as DestinationTestFilterSrc).I1 != 666)));

		var dest = mapper.Map(new DestinationTestFilterSrc());

		dest.I1.ShouldBe(13);
		dest.I2.ShouldBe(-5);
		dest.I3.ShouldBe(0);
		dest.L1.ShouldBe(0);
		dest.Str.ShouldBeNull();

		dest = mapper.Map(new DestinationTestFilterSrc { I1 = 666 }, new DestinationTestFilterDest());
		dest.I1.ShouldBe(0);
	}

	/// <summary>
	///   Tests the inheritence.
	/// </summary>
	[Fact]
	public void TestInheritence()
	{
		var mapper = Mapper.Default.GetMapper<BaseSource, InherDestination>();
		var dest = mapper.Map(new DerivedSource { I1 = 1, I2 = 2, I3 = 3, I4 = 4 });

		dest.I1.ShouldBe(1);
		dest.I2.ShouldBe(2);
		dest.I3.ShouldBe(3);
	}

	/// <summary>
	///   The base source.
	/// </summary>
	public class BaseSource
	{
		public int I1;
		public int I2;
		public int I3;
	}

	/// <summary>
	///   The derived.
	/// </summary>
	public class Derived : IDerived
	{
		/// <summary>
		///   Gets or Sets the base property.
		/// </summary>
		public string BaseProperty { get; set; }
		/// <summary>
		///   Gets or Sets the derived property.
		/// </summary>
		public string DerivedProperty { get; set; }
	}

	/// <summary>
	///   The derived source.
	/// </summary>
	public class DerivedSource : BaseSource
	{
		public int I4;
	}

	/// <summary>
	///   The destination test filter dest.
	/// </summary>
	public class DestinationTestFilterDest
	{
		public int I1;
		public int I2 = -5;
		public int I3 = 0;
		public long L1;
		public string Str;
	}

	/// <summary>
	///   The destination test filter src.
	/// </summary>
	public class DestinationTestFilterSrc
	{
		public int I1 = 13;
		public int I2 = 14;
		public int I3 = 5;
		public long L1 = 5;
		public string Str = "hello";
	}

	/// <summary>
	///   The inher destination.
	/// </summary>
	public class InherDestination
	{
		public int I1;
		public int I2;
		public int I3;
	}

	/// <summary>
	///   The target.
	/// </summary>
	public class Target
	{
		/// <summary>
		///   Gets or Sets the base property.
		/// </summary>
		public string BaseProperty { get; set; }
		/// <summary>
		///   Gets or Sets the derived property.
		/// </summary>
		public string DerivedProperty { get; set; }
	}

	/// <summary>
	///   The base interface.
	/// </summary>
	public interface IBase
	{
		/// <summary>
		///   Gets or Sets the base property.
		/// </summary>
		string BaseProperty { get; set; }
	}

	/// <summary>
	///   The derived interface.
	/// </summary>
	public interface IDerived : IBase
	{
		/// <summary>
		///   Gets or Sets the derived property.
		/// </summary>
		string DerivedProperty { get; set; }
	}
}