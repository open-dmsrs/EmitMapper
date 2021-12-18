using System.Linq;
using LightDataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmitMapper.Samples.SamplesTests;

[TestClass]
public class TestObjectsTracker
{
    [TestMethod]
    public void TestObjectsTracking()
    {
        var tracker = new ObjectsChangeTracker();
        var a = new A();
        tracker.RegisterObject(a);
        a.F2 = 3;
        var changes = tracker.GetChanges(a);
        Assert.IsTrue(changes[0].Name == "f2");
        tracker.RegisterObject(a);
        changes = tracker.GetChanges(a);
        Assert.IsTrue(changes.Length == 0);

        a.F1 = "new";
        a.F2 = 13;
        a.F3 = false;
        for (var i = 0; i < 10; ++i) tracker.GetChanges(a);

        changes = tracker.GetChanges(a);
        Assert.IsTrue(TestUtils.AreEqual(new[] { "f1", "f2", "f3" }, changes.Select(c => c.Name).ToArray()));

        changes = tracker.GetChanges(new A());
        Assert.IsNull(changes);
    }

    [TestMethod]
    public void TestTwoVersionOfObjects()
    {
        var tracker = new ObjectsChangeTracker();
        var original = new A { F1 = "f1", F2 = 2, F3 = true };
        var current = new A { F1 = "f0", F2 = 2, F3 = false };
        var changes = tracker.GetChanges(original, current);

        Assert.AreEqual(2, changes.Count());
        Assert.AreEqual("f1", changes[0].Name);
        Assert.AreEqual("f0", changes[0].CurrentValue);
        Assert.AreEqual("f3", changes[1].Name);
        Assert.AreEqual(false, changes[1].CurrentValue);

        changes = tracker.GetChanges(original, null);
        Assert.IsNull(changes);

        changes = tracker.GetChanges(null, current);
        Assert.IsNull(changes);
    }

    public class A
    {
        public bool F3 = true;
        public int F2 = 2;
        public string F1 = "f1";
    }
}