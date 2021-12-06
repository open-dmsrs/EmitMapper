using EmitMapperTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace LightDataAccess
{
    [TestClass]
    public class TestObjectsTracker
    {
        public class A
        {
            public string f1 = "f1";
            public int f2 = 2;
            public bool f3 = true;
        }

        [TestMethod]
        public void TestObjectsTracking()
        {
            ObjectsChangeTracker tracker = new ObjectsChangeTracker();
            A a = new A();
            tracker.RegisterObject(a);
            a.f2 = 3;
            ObjectsChangeTracker.TrackingMember[] changes = tracker.GetChanges(a);
            Assert.IsTrue(changes[0].name == "f2");
            tracker.RegisterObject(a);
            changes = tracker.GetChanges(a);
            Assert.IsTrue(changes.Length == 0);

            a.f1 = "new";
            a.f2 = 13;
            a.f3 = false;
            for (int i = 0; i < 10; ++i)
            {
                tracker.GetChanges(a);
            }

            changes = tracker.GetChanges(a);
            Assert.IsTrue(TestUtils.AreEqualArraysUnordered(new[] { "f1", "f2", "f3" }, changes.Select(c => c.name).ToArray()));

            changes = tracker.GetChanges(new A());
            Assert.IsNull(changes);
        }
        [TestMethod]
        public void TestTwoVersionOfObjects()
        {
            ObjectsChangeTracker tracker = new ObjectsChangeTracker();
            A original = new A() { f1 = "f1", f2 = 2, f3 = true };
            A current = new A() { f1 = "f0", f2 = 2, f3 = false };
            ObjectsChangeTracker.TrackingMember[] changes = tracker.GetChanges(original, current);

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
    }
}
