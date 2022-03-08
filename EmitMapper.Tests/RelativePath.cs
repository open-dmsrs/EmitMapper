using Shouldly;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace EmitMapper.Tests;
/// <summary>
/// The relative path.
/// </summary>

public class RelativePath
{
  private readonly ITestOutputHelper outputHelper;

  public RelativePath(ITestOutputHelper outputHelper)
  {
    Console.WriteLine(outputHelper.GetType().FullName);


    this.outputHelper = outputHelper;

  }

  /// <summary>
  /// Gets the relative path.
  /// </summary>
  [Fact]
  public void GetRelativePath()
  {

    var parent = Directory.GetCurrentDirectory();
    var sub = Path.Combine(parent, @"Users/Default");

    outputHelper.WriteLine(sub);
    outputHelper.WriteLine(parent);
    var result = Path.GetRelativePath(parent, sub);
    result.ShouldBe(Path.Combine("Users", "Default"));
    result = Path.GetRelativePath(sub, parent);
    result.ShouldBe(@".." + Path.DirectorySeparatorChar + "..");
  }

}

