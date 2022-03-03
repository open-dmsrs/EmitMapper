using Shouldly;
using Xunit;

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
    public En2 En2;
    public decimal En4;
    public string En5;
    public En1? En6;
    public En3 En7;
    public En3? En8;
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
    public decimal En1 = 3;
    public string En3 = "C";
    public En2 En4 = EnumTests.En2.B;
    public En3 En5 = EnumTests.En3.A;
    public En2 En6 = EnumTests.En2.C;
    public En1? En7 = EnumTests.En1.C;
    public En1? En8 = EnumTests.En1.C;
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
    A = 1,
    B = 2,
    C = 3
  }

  /// <summary>
  ///   The en2.
  /// </summary>
  public enum En2 : long
  {
    A = 1,
    B = 2,
    C = 3
  }

  /// <summary>
  ///   The en3.
  /// </summary>
  public enum En3
  {
    B = 2,
    C = 3,
    A = 1
  }
}