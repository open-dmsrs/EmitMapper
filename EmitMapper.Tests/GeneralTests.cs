namespace EmitMapper.Tests;

/// <summary>
///   The general tests.
/// </summary>
public class GeneralTests
{
	/// <summary>
	///   Constructs the by test.
	/// </summary>
	[Fact]
	public void ConstructByTest()
	{
		var mapper = Mapper.Default.GetMapper<ConstructBySource, ConstructByDestination>(
		  new DefaultMapConfig().ConstructBy(() => new ConstructByDestination.NestedClass(3))
			.ConstructBy(() => new ConstructByDestination(-1)));

		var d = mapper.Map(new ConstructBySource());
		d.Field.Str.ShouldBe("ConstructBy_Source::str");
		d.Field.I.ShouldBe(3);
	}

	/// <summary>
	///   Constructs the by test2.
	/// </summary>
	[Fact]
	public void ConstructByTest2()
	{
		var mapper = Mapper.Default.GetMapper<string, Guid>(
		  new DefaultMapConfig().ConvertUsing<string, Guid>(s => new Guid(s)));

		var guid = Guid.NewGuid();
		var d = mapper.Map(guid.ToString());
		guid.ShouldBe(d);
	}

	/// <summary>
	///   Generals the tests_ convert using.
	/// </summary>
	[Fact]
	public void GeneralTestsConvertUsing()
	{
		var a = Mapper.Default.GetMapper<B2, A2>(
		  new DefaultMapConfig().ConvertUsing<string, string>(s => "converted " + s)).Map(new B2());

		a.Str.ShouldBe("converted str");
	}

	/// <summary>
	///   Generals the tests_ example2.
	/// </summary>
	[Fact]
	public void GeneralTestsExample2()
	{
		var mapper = Mapper.Default.GetMapper<Source, Destination>(
		  new DefaultMapConfig().MatchMembers((m1, m2) => "M" + m1 == m2));

		var src = new Source();
		var dst = mapper.Map(src);
		src.Field1.ShouldBe(dst.MField1);
		src.Field2.ShouldBe(dst.MField2);
		src.Field3.ShouldBe(dst.MField3);
	}

	/// <summary>
	///   Generals the tests_ exception.
	/// </summary>
	[Fact]
	public void GeneralTestsException()
	{
		var mapper = Mapper.Default.GetMapper<B3, A3>();

		// DynamicAssemblyManager.SaveAssembly();
		var a = mapper.Map(new B3());
		a.ShouldNotBeNull();
		a.I1.ShouldBeNull();
		a.I2.ShouldNotBeNull();
		a.I3.ShouldNotBeNull();
		a.I2.I1.ShouldNotBeNull();
		a.I2.I2.ShouldNotBeNull();
		a.I2.I3.ShouldBeNull();
		a.I3.I1.ShouldNotBeNull();
		a.I3.I2.ShouldNotBeNull();
		a.I3.I3.ShouldBeNull();

		a.I2.I1.Str1.ShouldBe("1");
		a.I2.I2.Str1.ShouldBe("1");
		a.I3.I1.Str1.ShouldBe("1");
		a.I3.I2.Str1.ShouldBe("1");
		a.I2.I1.Str2.ShouldBeNull();
		a.I2.I2.Str2.ShouldBeNull();
		a.I3.I1.Str2.ShouldBeNull();
		a.I3.I2.Str2.ShouldBeNull();
		a.I2.I1.I.ShouldBe(10);
		a.I2.I2.I.ShouldBe(10);
		a.I3.I1.I.ShouldBe(10);
		a.I3.I2.I.ShouldBe(10);
	}

	/// <summary>
	///   Generals the tests_ ignore.
	/// </summary>
	[Fact]
	public void GeneralTestsIgnore()
	{
		var a = Mapper.Default.GetMapper<B, A>(new DefaultMapConfig().IgnoreMembers<B, A>("Str1"))
		  .Map(new B());

		a.Str1.ShouldBe("Destination::str1");
		A.EnType.En2.ShouldBe(a.En);
	}

	/// <summary>
	///   Generals the tests_ test1.
	/// </summary>
	[Fact]
	public void GeneralTestsTest1()
	{
		var a = new A();
		var b = new B();
		Mapper<B?, A?> mapper = Mapper.Default.GetMapper<B, A>(new DefaultMapConfig().DeepMap());

		// DynamicAssemblyManager.SaveAssembly();
		mapper.Map(b, a);
		A.EnType.En2.ShouldBe(a.En);
		a.Str1.ShouldBe(b.Str1);
		a.Str2.ShouldBe(b.Str2);
		a.Obj.Str.ShouldBe(b.Obj.Str);
		a.Obj.Intern.ShouldBe(13);
		a.Arr.Length.ShouldBe(b.Arr.Length);
		a.Arr[0].ShouldBe(b.Arr[0]);
		a.Arr[1].ShouldBe(b.Arr[1]);
		a.Arr[2].ShouldBe(b.Arr[2]);

		a.ObjArr.Length.ShouldBe(b.ObjArr.Length);
		b.ObjArr[0].Str.ShouldBe(a.ObjArr[0].Str);
		a.ObjArr[1].Str.ShouldBe(b.ObjArr[1].Str);
		a.Str3.ShouldBeNull();
	}

	/// <summary>
	///   Generals the tests_ test2.
	/// </summary>
	[Fact]
	public void GeneralTestsTest2()
	{
		var a = new A();
		var b = new B();

		a.Obj = new A.AInt();
		b.Obj = null;

		Mapper<B?, A?> mapper = Context.ObjMan.GetMapper<B, A>();

		// DynamicAssemblyManager.SaveAssembly();
		mapper.Map(b, a);
		a.Obj.ShouldBeNull();
	}

	/// <summary>
	///   Generals the tests_ test3.
	/// </summary>
	[Fact]
	public void GeneralTestsTest3()
	{
		var a = new A();
		var b = new B();
		a.Obj = new A.AInt { Intern = 15 };

		Mapper<B?, A?> mapper = Context.ObjMan.GetMapper<B, A>();
		mapper.Map(b, a);
		a.Obj.Intern.ShouldBe(15);
	}

	/// <summary>
	///   Simples the test.
	/// </summary>
	[Fact]
	public void SimpleTest()
	{
		var mapper = Mapper.Default.GetMapper<Simple2, Simple1>();

		// DynamicAssemblyManager.SaveAssembly();
		var s = mapper.Map(new Simple2());
		s.I.ShouldBe(20);
		A.EnType.En2.ShouldBe(s.Fld1);
	}

	/// <summary>
	///   Simples the test class.
	/// </summary>
	[Fact]
	public void SimpleTestClass()
	{
		var mapper = Context.ObjMan.GetMapper<Class2, Class1>();

		// DynamicAssemblyManager.SaveAssembly();
		var s = mapper.Map(new Class2 { Fld = 13 });
		s.Fld.ShouldBe(13);
	}

	/// <summary>
	///   Simples the test enum.
	/// </summary>
	[Fact]
	public void SimpleTestEnum()
	{
		var mapper = Context.ObjMan.GetMapper<B.EnType, A.EnType>();

		// DynamicAssemblyManager.SaveAssembly();
		var aen = mapper.Map(B.EnType.En3);
		A.EnType.En3.ShouldBe(aen);
	}

	/// <summary>
	///   Simples the test struct.
	/// </summary>
	[Fact]
	public void SimpleTestStruct()
	{
		var mapper = Context.ObjMan.GetMapper<Struct2, Struct1>();

		// DynamicAssemblyManager.SaveAssembly();
		var s = mapper.Map(new Struct2 { Fld = 13 });
		s.Fld.ShouldBe(13);
	}

	/// <summary>
	///   Tests the recursive class.
	/// </summary>
	[Fact]
	public void TestRecursiveClass()
	{
		var tree = new TreeNode
		{
			Data = "node 1",
			Next = new TreeNode
			{
				Data = "node 2",
				Next = new TreeNode
				{
					Data = "node 3",
					SubNodes = new[] { new TreeNode { Data = "sub sub data 1" }, new TreeNode { Data = "sub sub data 2" } }
				}
			},
			SubNodes = new[] { new TreeNode { Data = "sub data 1" } }
		};

		var mapper = Mapper.Default.GetMapper<TreeNode, TreeNode>(new DefaultMapConfig().DeepMap());
		var tree2 = mapper.Map(tree);
		tree2.Data.ShouldBe("node 1");
		tree2.Next.Data.ShouldBe("node 2");
		tree2.Next.Next.Data.ShouldBe("node 3");
		tree2.SubNodes[0].Data.ShouldBe("sub data 1");
		tree2.Next.Next.SubNodes[0].Data.ShouldBe("sub sub data 1");
		tree2.Next.Next.SubNodes[1].Data.ShouldBe("sub sub data 2");
		tree2.Next.Next.Next.ShouldBeNull();
	}

	/// <summary>
	///   The a.
	/// </summary>
	public class A
	{
		/// <summary>
		/// En
		/// </summary>
		public EnType En = EnType.En3;

		/// <summary>
		/// Obj
		/// </summary>
		public AInt Obj;

		/// <summary>
		/// Obj arr
		/// </summary>
		public AInt[] ObjArr;

		/// <summary>
		/// Str2
		/// </summary>
		public string Str2 = "Destination::str2";

		/// <summary>
		/// Str3
		/// </summary>
		public string Str3 = "Destination::str3";

		/// <summary>
		///   Initializes a new instance of the <see cref="A" /> class.
		/// </summary>
		public A()
		{
			Console.WriteLine("Destination::Destination()");
		}

		/// <summary>
		///   The en type.
		/// </summary>
		public enum EnType
		{
			/// <summary>
			/// En1
			/// </summary>
			En1,

			/// <summary>
			/// En2
			/// </summary>
			En2,

			/// <summary>
			/// En3
			/// </summary>
			En3
		}

		/// <summary>
		///   Gets or Sets the arr.
		/// </summary>
		public int[] Arr { get; set; }

		/// <summary>
		///   Gets or Sets the str1.
		/// </summary>
		public string Str1 { get; set; } = "Destination::str1";

		/// <summary>
		///   The a int.
		/// </summary>
		public class AInt
		{
			/// <summary>
			/// Str
			/// </summary>
			public string Str = "AInt";

			/// <summary>
			/// Intern
			/// </summary>
			internal int Intern = 13;

			/// <summary>
			///   Initializes a new instance of the <see cref="AInt" /> class.
			/// </summary>
			public AInt()
			{
				Intern = 13;
			}
		}
	}

	/// <summary>
	///   The a1.
	/// </summary>
	internal class A1
	{
		/// <summary>
		/// F1
		/// </summary>
		public string F1 = "A1::f1";

		/// <summary>
		/// F2
		/// </summary>
		public string F2 = "A1::f2";
	}

	/// <summary>
	///   The a2.
	/// </summary>
	public class A2
	{
		/// <summary>
		/// Str
		/// </summary>
		public string? Str;
	}

	/// <summary>
	///   The a3.
	/// </summary>
	public class A3
	{
		/// <summary>
		/// I1
		/// </summary>
		public Int2? I1;

		/// <summary>
		/// I2
		/// </summary>
		public Int2? I2;

		/// <summary>
		/// I3
		/// </summary>
		public Int2? I3;

		/// <summary>
		///   The int1.
		/// </summary>
		public class Int1
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

		/// <summary>
		///   The int2.
		/// </summary>
		public class Int2
		{
			/// <summary>
			/// I1
			/// </summary>
			public Int1? I1;

			/// <summary>
			/// I2
			/// </summary>
			public Int1? I2;

			/// <summary>
			/// I3
			/// </summary>
			public Int1? I3;
		}
	}

	/// <summary>
	///   The b.
	/// </summary>
	public class B
	{
		/// <summary>
		/// En
		/// </summary>
		public EnType En = EnType.En2;

		/// <summary>
		/// Obj
		/// </summary>
		public BInt Obj = new();

		/// <summary>
		/// Obj arr
		/// </summary>
		public BInt[] ObjArr;

		/// <summary>
		/// Str1
		/// </summary>
		public string Str1 = "Source::str1";

		/// <summary>
		/// Str3
		/// </summary>
		public object Str3 = null;

		/// <summary>
		///   Initializes a new instance of the <see cref="B" /> class.
		/// </summary>
		public B()
		{
			Console.WriteLine("Source::Source()");

			ObjArr = new BInt[2];
			ObjArr[0] = new BInt { Str = "b objArr 1" };
			ObjArr[1] = new BInt { Str = "b objArr 2" };
		}

		/// <summary>
		///   The en type.
		/// </summary>
		public enum EnType
		{
			/// <summary>
			/// En1
			/// </summary>
			En1,

			/// <summary>
			/// En2
			/// </summary>
			En2,

			/// <summary>
			/// En3
			/// </summary>
			En3
		}

		/// <summary>
		///   Gets the arr.
		/// </summary>
		public int[] Arr => new[] { 1, 5, 9 };

		/// <summary>
		///   Gets the str2.
		/// </summary>
		public string Str2 => "Source::str2";

		/// <summary>
		///   The b int.
		/// </summary>
		public class BInt
		{
			/// <summary>
			/// Str
			/// </summary>
			public string Str = "BInt";
			/*
				  public string str
				  {
					  get
					  {
						  throw new Exception("reading BInt::str");
					  }
					  set { }
				  }
				   */
		}
	}

	/// <summary>
	///   The b1.
	/// </summary>
	internal class B1
	{
		/// <summary>
		/// F1
		/// </summary>
		public string F1 = "B1::f1";

		/// <summary>
		/// F2
		/// </summary>
		public string F2 = "B1::f2";
	}

	/// <summary>
	///   The b2.
	/// </summary>
	public class B2
	{
		/// <summary>
		/// Str
		/// </summary>
		public string Str = "str";
	}

	/// <summary>
	///   The b3.
	/// </summary>
	public class B3
	{
		/// <summary>
		/// I1
		/// </summary>
		public Int2 I1 = null;

		/// <summary>
		/// I2
		/// </summary>
		public Int2 I2 = new();

		/// <summary>
		/// I3
		/// </summary>
		public Int2 I3 = new();

		/// <summary>
		///   The int1.
		/// </summary>
		public class Int1
		{
			/// <summary>
			/// I
			/// </summary>
			public long I = 10;

			/// <summary>
			/// Str1
			/// </summary>
			public string Str1 = "1";

			/// <summary>
			/// Str2
			/// </summary>
			public string Str2 = null;
		}

		/// <summary>
		///   The int2.
		/// </summary>
		public class Int2
		{
			/// <summary>
			/// I1
			/// </summary>
			public Int1 I1 = new();

			/// <summary>
			/// I2
			/// </summary>
			public Int1 I2 = new();

			/// <summary>
			/// I3
			/// </summary>
			public Int1 I3 = null;
		}
	}

	/// <summary>
	///   The construct by destination.
	/// </summary>
	public class ConstructByDestination
	{
		/// <summary>
		/// Field
		/// </summary>
		public NestedClass Field;

		/// <summary>
		///   Initializes a new instance of the <see cref="ConstructByDestination" /> class.
		/// </summary>
		/// <param name="i">The i.</param>
		public ConstructByDestination(int i)
		{
		}

		/// <summary>
		///   The nested class.
		/// </summary>
		public class NestedClass
		{
			/// <summary>
			/// I
			/// </summary>
			public int I;

			/// <summary>
			/// Str
			/// </summary>
			public string Str;

			/// <summary>
			///   Initializes a new instance of the <see cref="NestedClass" /> class.
			/// </summary>
			/// <param name="i">The i.</param>
			public NestedClass(int i)
			{
				Str = "ConstructBy_Destination::str";
				I = i;
			}
		}
	}

	/// <summary>
	///   The construct by source.
	/// </summary>
	public class ConstructBySource
	{
		/// <summary>
		/// Field
		/// </summary>
		public NestedClass Field = new();

		/// <summary>
		///   The nested class.
		/// </summary>
		public class NestedClass
		{
			/// <summary>
			/// Str
			/// </summary>
			public string Str = "ConstructBy_Source::str";
		}
	}

	/// <summary>
	///   The destination.
	/// </summary>
	public class Destination
	{
		/// <summary>
		/// M field1
		/// </summary>
		public string? MField1;

		/// <summary>
		/// M field2
		/// </summary>
		public string? MField2;

		/// <summary>
		/// M field3
		/// </summary>
		public string? MField3;
	}

	/// <summary>
	///   The simple1.
	/// </summary>
	public class Simple1
	{
		/// <summary>
		/// Fld1
		/// </summary>
		public A.EnType Fld1 = A.EnType.En1;

		/// <summary>
		/// I
		/// </summary>
		public int I = 10;
	}

	/// <summary>
	///   The simple2.
	/// </summary>
	public class Simple2
	{
		/// <summary>
		/// Fld1
		/// </summary>
		public B.EnType Fld1 = B.EnType.En2;

		/// <summary>
		/// I
		/// </summary>
		public int I = 20;
	}

	/// <summary>
	///   The source.
	/// </summary>
	public class Source
	{
		/// <summary>
		/// Field1
		/// </summary>
		public string Field1 = "Source::field1";

		/// <summary>
		/// Field2
		/// </summary>
		public string Field2 = "Source::field2";

		/// <summary>
		/// Field3
		/// </summary>
		public string Field3 = "Source::field3";
	}

	/// <summary>
	///   The tree node.
	/// </summary>
	public class TreeNode
	{
		/// <summary>
		/// Data
		/// </summary>
		public string? Data;

		/// <summary>
		/// Next
		/// </summary>
		public TreeNode? Next;

		/// <summary>
		/// Sub nodes
		/// </summary>
		public TreeNode[]? SubNodes;
	}

	/// <summary>
	/// Class1
	/// </summary>
	public struct Class1
	{
		/// <summary>
		/// Fld
		/// </summary>
		public int Fld;
	}

	/// <summary>
	/// Class2
	/// </summary>
	public struct Class2
	{
		/// <summary>
		/// Fld
		/// </summary>
		public int Fld;
	}

	/// <summary>
	/// Struct1
	/// </summary>
	public struct Struct1
	{
		/// <summary>
		/// Fld
		/// </summary>
		public int Fld;
	}

	/// <summary>
	/// Struct2
	/// </summary>
	public struct Struct2
	{
		/// <summary>
		/// Fld
		/// </summary>
		public int Fld;
	}
}