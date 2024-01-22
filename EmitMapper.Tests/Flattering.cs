namespace EmitMapper.Tests;

/// <summary>
///   The flattering.
/// </summary>
public class Flattering
{
	/// <summary>
	///   Tests the flattering1.
	/// </summary>
	[Fact]
	public void TestFlattering1()
	{
		var rw1 = new ReadWriteSimple
		{
			Source = new MemberDescriptor(
			new[]
			{
		  typeof(Source).GetMember(nameof(Source.InnerSource))[0],
		  typeof(Source.InnerSourceClass).GetMember(nameof(Source.InnerSource.Message))[0]
			}),
			Destination = new MemberDescriptor(
			new[] { typeof(Destination).GetMember(nameof(Destination.Message))[0] })
		};

		var rw2 = new ReadWriteSimple
		{
			Source = new MemberDescriptor(
			new[]
			{
		  typeof(Source).GetMember(nameof(Source.InnerSource))[0],
		  typeof(Source.InnerSourceClass).GetMember(nameof(Source.InnerSourceClass.GetMessage2))[0]
			}),
			Destination = new MemberDescriptor(
			new[] { typeof(Destination).GetMember(nameof(Destination.Message2))[0] })
		};

		var mapper = Mapper.Default.GetMapper<Source, Destination>(
		  new CustomMapConfig { GetMappingOperationFunc = (from, to) => rw1.AsEnumerable(rw2) });

		var b = new Source();
		var result = mapper.Map(b);
		b.InnerSource.Message.ShouldBe(result.Message);
		b.InnerSource.GetMessage2().ShouldBe(result.Message2);
	}

	/// <summary>
	///   The destination.
	/// </summary>
	public class Destination
	{
		/// <summary>
		/// Message
		/// </summary>
		public string? Message;

		/// <summary>
		/// Message2
		/// </summary>
		public string? Message2;
	}

	/// <summary>
	///   The source.
	/// </summary>
	public class Source
	{
		/// <summary>
		/// Inner source
		/// </summary>
		public InnerSourceClass InnerSource = new();

		/// <summary>
		///   The inner source class.
		/// </summary>
		public class InnerSourceClass
		{
			/// <summary>
			/// Message
			/// </summary>
			public string Message = "message's value";

			/// <summary>
			///   Gets the message2.
			/// </summary>
			/// <returns>A string.</returns>
			public string GetMessage2()
			{
				return "GetMessage2 's value";
			}
		}
	}
}