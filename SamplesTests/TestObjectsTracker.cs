using System.Linq;
using EmitMapper.Samples.SamplesTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightDataAccess;

[TestClass]
public class TestObjectsTracker
{
    [TestMethod]
    public void TestObjectsTracking()
    {
        var tracker = new ObjectsChangeTracker();
        var a = new A();
        tracker.RegisterObject(a);
        a.f2 = 3;
        var changes = tracker.GetChanges(a);
        Assert.IsTrue(changes[0].name == "f2");
        tracker.RegisterObject(a);
        changes = tracker.GetChanges(a);
        Assert.IsTrue(changes.Length == 0);

        a.f1 = "new";
        a.f2 = 13;
        a.f3 = false;
        for (var i = 0; i < 10; ++i) tracker.GetChanges(a);

        changes = tracker.GetChanges(a);
        Assert.IsTrue(TestUtils.AreEqual(new[] { "f1", "f2", "f3" }, changes.Select(c => c.name).ToArray()));

        changes = tracker.GetChanges(new A());
        Assert.IsNull(changes);
    }

    [TestMethod]
    public void TestTwoVersionOfObjects()
    {
        var tracker = new ObjectsChangeTracker();
        var original = new A { f1 = "f1", f2 = 2, f3 = true };
        var current = new A { f1 = "f0", f2 = 2, f3 = false };
        var changes = tracker.GetChanges(original, current);

        Assert.AreEqual(2, changes.Count());
        Assert.AreEqual("f1", changes[0].name);
        Assert.AreEqual("f0", changes[0].CurrentValue);
        Assert.AreEqual("f3", changes[1].name);
        Assert.AreEqual(false, changes[1].CurrentValue);

        changes = tracker.GetChanges(original, null);
        Assert.IsNull(changes);

        changes = tracker.GetChanges(null, current);
        Assert.IsNull(changes);
    }

    public class A
    {
        public bool f3 = true;
        public int f2 = 2;
        public string f1 = "f1";
    }
}