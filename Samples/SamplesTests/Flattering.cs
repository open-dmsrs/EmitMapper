using EMConfigurations;

namespace SamplesTests;

/// <summary>
///   The flattering.
/// </summary>
public class Flattering
{
	/// <summary>
	///   Tests the flattering.
	/// </summary>
	[Fact]
	public void TestFlattering()
	{
		var source = new ModelObject
		{
			BaseDate = DateTime.Now,
			Sub = new ModelSubObject
			{
				ProperName = "Some name",
				SubSub = new ModelSubSubObject { AmACoolProperty = "Cool daddy-o" }
			},
			Sub2 = new ModelSubObject { ProperName = "Sub 2 name" },
			SubWithExtraName = new ModelSubObject { ProperName = "Some other name" }
		};

		var mapper = Mapper.Default.GetMapper<ModelObject, ModelDto>(
		  new FlatteringConfig().IgnoreMembers<ModelObject, ModelDto>("SubSubSubNoExistInSourceProperty"));

		var b = mapper.Map(source);

		b.BaseDate.ShouldBe(source.BaseDate);
		b.TestMethod1.ShouldBe(source.TestMethod1());
		b.SubProperName.ShouldBe(source.Sub.ProperName);
		b.SubSubSubAmACoolProperty.ShouldBe(source.Sub.SubSub.AmACoolProperty);
		b.Sub2ProperName.ShouldBe(source.Sub2.ProperName);
		b.SubWithExtraNameProperName.ShouldBe(source.SubWithExtraName.ProperName);
	}

	/// <summary>
	///   The model dto.
	/// </summary>
	public class ModelDto
	{
		public string? TestMethod1;

		/// <summary>
		///   Gets or Sets the base date.
		/// </summary>
		public DateTime BaseDate { get; set; }

		/// <summary>
		///   Gets or Sets the sub2 proper name.
		/// </summary>
		public string? Sub2ProperName { get; set; }

		/// <summary>
		///   Gets or Sets the sub proper name.
		/// </summary>
		public string? SubProperName { get; set; }

		/// <summary>
		///   Gets or Sets the sub sub sub i am a cool property.
		/// </summary>
		public string? SubSubSubAmACoolProperty { get; set; }

		/// <summary>
		///   Gets or Sets the sub with extra name proper name.
		/// </summary>
		public string? SubWithExtraNameProperName { get; set; }

		/// <summary>
		///   cant support this property
		/// </summary>

		// public string SubSubSubNoExistInSourceProperty { get; set; }
		public void T()
		{
		}
	}

	/// <summary>
	///   The model object.
	/// </summary>
	public class ModelObject
	{
		/// <summary>
		///   Gets or Sets the base date.
		/// </summary>
		public DateTime BaseDate { get; set; }

		/// <summary>
		///   Gets or Sets the sub.
		/// </summary>
		public ModelSubObject? Sub { get; set; }

		/// <summary>
		///   Gets or Sets the sub2.
		/// </summary>
		public ModelSubObject? Sub2 { get; set; }

		/// <summary>
		///   Gets or Sets the sub with extra name.
		/// </summary>
		public ModelSubObject? SubWithExtraName { get; set; }

		/// <summary>
		///   Tests the method.
		/// </summary>
		public void TestMethod()
		{
		}

		/// <summary>
		///   Tests the method1.
		/// </summary>
		/// <returns>A string.</returns>
		public string TestMethod1()
		{
			return "1";
		}
	}

	/// <summary>
	///   The model sub object.
	/// </summary>
	public class ModelSubObject
	{
		/// <summary>
		///   Gets or Sets the proper name.
		/// </summary>
		public string? ProperName { get; set; }

		/// <summary>
		///   Gets or Sets the sub sub.
		/// </summary>
		public ModelSubSubObject? SubSub { get; set; }

		/// <summary>
		///   Tests the method.
		/// </summary>
		public void TestMethod()
		{
		}

		/// <summary>
		///   Tests the method1.
		/// </summary>
		/// <returns>A string.</returns>
		public string TestMethod1()
		{
			return "1";
		}
	}

	/// <summary>
	///   The model sub sub object.
	/// </summary>
	public class ModelSubSubObject
	{
		/// <summary>
		///   Gets or Sets the i am a cool property.
		/// </summary>
		public string? AmACoolProperty { get; set; }
	}
}