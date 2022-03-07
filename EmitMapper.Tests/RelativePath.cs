using Shouldly;
using System.IO;
using Xunit;

namespace EmitMapper.Tests;
/// <summary>
/// The relative path.
/// </summary>

public class RelativePath
{
  /// <summary>
  /// Gets the relative path.
  /// </summary>
  [Fact]
  public void GetRelativePath()
  {
    //var parent = Path.GetPathRoot(@"\");
    //var sub = Path.GetPathRoot(@"\Users\Default");
    //var result = Path.GetRelativePath(parent, sub);
    //result.ShouldBe(@"Users\Default");

    //result = Path.GetRelativePath(sub, parent);
    //result.ShouldBe(@"..\..");
  }

}