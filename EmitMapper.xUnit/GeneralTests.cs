using System;
using EmitMapper.MappingConfiguration;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

////[TestFixture]
public class GeneralTests
{
  [Fact]
  public void ConstructByTest()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ConstructBySource, ConstructByDestination>(
      new DefaultMapConfig().ConstructBy(() => new ConstructByDestination.NestedClass(3))
        .ConstructBy(() => new ConstructByDestination(-1)));

    var d = mapper.Map(new ConstructBySource());
    d.Field.Str.ShouldBe("ConstructBy_Source::str");
    d.Field.I.ShouldBe(3);
  }

  [Fact]
  public void ConstructByTest2()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<string, Guid>(
      new DefaultMapConfig().ConvertUsing<string, Guid>(s => new Guid(s)));

    var guid = Guid.NewGuid();
    var d = mapper.Map(guid.ToString());
    guid.ShouldBe(d);
  }

  [Fact]
  public void GeneralTests_ConvertUsing()
  {
    var a = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
      new DefaultMapConfig().ConvertUsing<string, string>(s => "converted " + s)).Map(new B2());

    a.Str.ShouldBe("converted str");
  }

  [Fact]
  public void GeneralTests_Example2()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>(
      new DefaultMapConfig().MatchMembers((m1, m2) => "M" + m1 == m2));

    var src = new Source();
    var dst = mapper.Map(src);
    src.Field1.ShouldBe(dst.MField1);
    src.Field2.ShouldBe(dst.MField2);
    src.Field3.ShouldBe(dst.MField3);
  }

  [Fact]
  public void GeneralTests_Exception()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();

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

  [Fact]
  public void GeneralTests_Ignore()
  {
    var a = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(new DefaultMapConfig().IgnoreMembers<B, A>("Str1"))
      .Map(new B());

    a.Str1.ShouldBe("Destination::str1");
    A.EnType.En2.ShouldBe(a.En);
  }

  [Fact]
  public void GeneralTests_Test1()
  {
    var a = new A();
    var b = new B();
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(new DefaultMapConfig().DeepMap());

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

  [Fact]
  public void GeneralTests_Test2()
  {
    var a = new A();
    var b = new B();

    a.Obj = new A.AInt();
    b.Obj = null;

    var mapper = Context.ObjMan.GetMapper<B, A>();

    // DynamicAssemblyManager.SaveAssembly();
    mapper.Map(b, a);
    a.Obj.ShouldBeNull();
  }

  [Fact]
  public void GeneralTests_Test3()
  {
    var a = new A();
    var b = new B();
    a.Obj = new A.AInt { Intern = 15 };

    var mapper = Context.ObjMan.GetMapper<B, A>();
    mapper.Map(b, a);
    a.Obj.Intern.ShouldBe(15);
  }

  [Fact]
  public void SimpleTest()
  {
    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Simple2, Simple1>();

    // DynamicAssemblyManager.SaveAssembly();
    var s = mapper.Map(new Simple2());
    s.I.ShouldBe(20);
    A.EnType.En2.ShouldBe(s.Fld1);
  }

  [Fact]
  public void SimpleTestClass()
  {
    var mapper = Context.ObjMan.GetMapper<Class2, Class1>();

    // DynamicAssemblyManager.SaveAssembly();
    var s = mapper.Map(new Class2 { Fld = 13 });
    s.Fld.ShouldBe(13);
  }

  [Fact]
  public void SimpleTestEnum()
  {
    var mapper = Context.ObjMan.GetMapper<B.EnType, A.EnType>();

    // DynamicAssemblyManager.SaveAssembly();
    var aen = mapper.Map(B.EnType.En3);
    A.EnType.En3.ShouldBe(aen);
  }

  [Fact]
  public void SimpleTestStruct()
  {
    var mapper = Context.ObjMan.GetMapper<Struct2, Struct1>();

    // DynamicAssemblyManager.SaveAssembly();
    var s = mapper.Map(new Struct2 { Fld = 13 });
    s.Fld.ShouldBe(13);
  }

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

    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TreeNode, TreeNode>(new DefaultMapConfig().DeepMap());
    var tree2 = mapper.Map(tree);
    tree2.Data.ShouldBe("node 1");
    tree2.Next.Data.ShouldBe("node 2");
    tree2.Next.Next.Data.ShouldBe("node 3");
    tree2.SubNodes[0].Data.ShouldBe("sub data 1");
    tree2.Next.Next.SubNodes[0].Data.ShouldBe("sub sub data 1");
    tree2.Next.Next.SubNodes[1].Data.ShouldBe("sub sub data 2");
    tree2.Next.Next.Next.ShouldBeNull();
  }

  public class A
  {
    public EnType En = EnType.En3;
    public AInt Obj;
    public AInt[] ObjArr;
    public string Str2 = "Destination::str2";
    public string Str3 = "Destination::str3";

    public A()
    {
      Console.WriteLine("Destination::Destination()");
    }

    public enum EnType
    {
      En1,
      En2,
      En3
    }

    public int[] Arr { get; set; }
    public string Str1 { get; set; } = "Destination::str1";

    public class AInt
    {
      public string Str = "AInt";
      internal int Intern = 13;

      public AInt()
      {
        Intern = 13;
      }
    }
  }

  internal class A1
  {
    public string F1 = "A1::f1";
    public string F2 = "A1::f2";
  }

  public class A2
  {
    public string Str;
  }

  public class A3
  {
    public Int2 I1;
    public Int2 I2;
    public Int2 I3;

    public class Int1
    {
      public int I;
      public string Str1;
      public string Str2;
    }

    public class Int2
    {
      public Int1 I1;
      public Int1 I2;
      public Int1 I3;
    }
  }

  public class B
  {
    public EnType En = EnType.En2;
    public BInt Obj = new();
    public BInt[] ObjArr;
    public string Str1 = "Source::str1";
    public object Str3 = null;

    public B()
    {
      Console.WriteLine("Source::Source()");

      ObjArr = new BInt[2];
      ObjArr[0] = new BInt { Str = "b objArr 1" };
      ObjArr[1] = new BInt { Str = "b objArr 2" };
    }

    public enum EnType
    {
      En1,
      En2,
      En3
    }

    public int[] Arr => new[] { 1, 5, 9 };
    public string Str2 => "Source::str2";

    public class BInt
    {
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

  internal class B1
  {
    public string F1 = "B1::f1";
    public string F2 = "B1::f2";
  }

  public class B2
  {
    public string Str = "str";
  }

  public class B3
  {
    public Int2 I1 = null;
    public Int2 I2 = new();
    public Int2 I3 = new();

    public class Int1
    {
      public long I = 10;
      public string Str1 = "1";
      public string Str2 = null;
    }

    public class Int2
    {
      public Int1 I1 = new();
      public Int1 I2 = new();
      public Int1 I3 = null;
    }
  }

  public class ConstructByDestination
  {
    public NestedClass Field;

    public ConstructByDestination(int i)
    {
    }

    public class NestedClass
    {
      public int I;
      public string Str;

      public NestedClass(int i)
      {
        Str = "ConstructBy_Destination::str";
        I = i;
      }
    }
  }

  public class ConstructBySource
  {
    public NestedClass Field = new();

    public class NestedClass
    {
      public string Str = "ConstructBy_Source::str";
    }
  }

  public class Destination
  {
    public string MField1;
    public string MField2;
    public string MField3;
  }

  public class Simple1
  {
    public A.EnType Fld1 = A.EnType.En1;
    public int I = 10;
  }

  public class Simple2
  {
    public B.EnType Fld1 = B.EnType.En2;
    public int I = 20;
  }

  public class Source
  {
    public string Field1 = "Source::field1";
    public string Field2 = "Source::field2";
    public string Field3 = "Source::field3";
  }

  public class TreeNode
  {
    public string Data;
    public TreeNode Next;
    public TreeNode[] SubNodes;
  }

  public struct Class1
  {
    public int Fld;
  }

  public struct Class2
  {
    public int Fld;
  }

  public struct Struct1
  {
    public int Fld;
  }

  public struct Struct2
  {
    public int Fld;
  }
}