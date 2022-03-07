using Shouldly;
using System.IO;
using Xunit;

namespace EmitMapper.Tests;

public class RelativePath
{
  [Fact]
  public void GetRelativePath()
  {
    var parent = @"c:\";
    var sub = @"C:\Users\Default";
    var result = Path.GetRelativePath(parent, sub);
    result.ShouldBe(@"Users\Default");

    result = Path.GetRelativePath(sub, parent);
    result.ShouldBe(@"..\..");
  }

}