using System.Linq;
using LightDataAccess;
using Xunit;

namespace SamplesTests;

using Shouldly;

public class TestObjectsTracker
{
  [Fact]
  public void Test_ObjectsChangeTracker_GetChanges_RegisterObject()
  {
    var tracker = new ObjectsChangeTracker();
    var a = new A();
    tracker.RegisterObject(a);
    a.F2 = 3;
    var changes = tracker.GetChanges(a);
    changes[0].Name.ShouldBe(nameof(A.F2));
    tracker.RegisterObject(a);
    changes = tracker.GetChanges(a);
    changes.Length.ShouldBe(0);

    a.F1 = "new";
    a.F2 = 13;
    a.F3 = false;
    for (var i = 0; i < 10; ++i)
      tracker.GetChanges(a);

    changes = tracker.GetChanges(a);
    

    changes.Select(c => c.Name).ShouldBe(new[] { "F1", "F2", "F3" });

    changes = tracker.GetChanges(new A());
    Assert.Null(changes);
  }

  [Fact]
  public void Test_ObjectsChangeTracker_GetChanges_two_objects()
  {
    var tracker = new ObjectsChangeTracker();
    var original = new A { F1 = "F1old", F2 = 2, F3 = true };
    var current = new A { F1 = "F1New", F2 = 2, F3 = false };
    var changes = tracker.GetChanges(original, current);

    changes.Count().ShouldBe(2);
    changes[1].Name.ShouldBe("F3");
    changes[1].CurrentValue.ShouldBe(false);
    changes[0].Name.ShouldBe("F1");
    changes[0].CurrentValue.ShouldBe("F1New");

    changes = tracker.GetChanges(original, null);
    Assert.Null(changes);

    changes = tracker.GetChanges(null, current);
    Assert.Null(changes);
  }

  public class A
  {
    public string F1 = "F1";

    public int F2 = 2;

    public bool F3 = true;
  }
}