using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapperTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert=Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
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
			var tracker = new ObjectsChangeTracker();
			var a = new A();
			tracker.RegisterObject(a);
			a.f2 = 3;
			var changes = tracker.GetChanges(a);
			Assert.IsTrue( changes[0].name == "f2");
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
            var tracker = new ObjectsChangeTracker();
            var first = new A (){ f1 = "f1", f2 = 2, f3 = true};
            var second = new A (){ f1 = "f0", f2 = 2, f3 = false};
            var changes = tracker.GetChanges(first, second);
            
            Assert.AreEqual(2, changes.Count());
            Assert.AreEqual("f1", changes[0].name);
            Assert.AreEqual("f0", changes[0].CurrentValue);
            Assert.AreEqual("f3", changes[1].name);
            Assert.AreEqual(false, changes[1].OriginalValue);
            
            changes = tracker.GetChanges(first, null);
            Assert.IsNull(changes);
            
            changes = tracker.GetChanges(null, second);
            Assert.IsNull(changes);
        }
	}
}
