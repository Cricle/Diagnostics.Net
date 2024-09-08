using Diagnostics.Traces.Stores;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class ConstDatabaseSelectorTest
    {
        [TestMethod]
        public void GivenNull_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConstDatabaseSelector<IDatabaseCreatedResult>(null));
        }

        [TestMethod]
        public void InitializersAndAfterSwitchedsWillNotSupport()
        {
            var selector = new ConstDatabaseSelector<DatabaseCreatedResult>(new DatabaseCreatedResult());

            Assert.ThrowsException<NotSupportedException>(() => _ = selector.AfterSwitcheds);
            Assert.ThrowsException<NotSupportedException>(() => _ = selector.Initializers);
        }

        [TestMethod]
        public void FlushWillFalse()
        {
            var selector = new ConstDatabaseSelector<DatabaseCreatedResult>(new DatabaseCreatedResult());

            Assert.IsFalse(selector.Flush());
        }

        [TestMethod]
        public void AllCallMustEqualsResult()
        {
            var result = new DatabaseCreatedResult();
            var selector = new ConstDatabaseSelector<DatabaseCreatedResult>(result);

            var called = false;
            DatabaseCreatedResult? actual = null;
            selector.UnsafeUsingDatabaseResult(r =>
            {
                called = true;
                actual = r;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            called = false;
            actual = null;

            var state = new object();
            object? actualState = null;
            selector.UnsafeUsingDatabaseResult(state,(r,s) =>
            {
                called = true;
                actual = r;
                actualState = s;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            Assert.AreEqual(state, actualState);
            called = false;
            actual = null;
            actualState = null;

            var res = selector.UnsafeUsingDatabaseResult(state, (r, s) =>
            {
                called = true;
                actual = r;
                actualState = s;
                return 1;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            Assert.AreEqual(state, actualState);
            Assert.AreEqual(1, res);
            called = false;
            actual = null;

            selector.UsingDatabaseResult((r) =>
            {
                called = true;
                actual = r;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            called = false;
            actual = null;

            selector.UsingDatabaseResult(state,(r,s) =>
            {
                called = true;
                actual = r;
                actualState = s;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            Assert.AreEqual(state, actualState);
            called = false;
            actual = null;
            actualState = null;

            res = selector.UsingDatabaseResult(state, (r, s) =>
            {
                called = true;
                actual = r;
                actualState = s;
                return 1;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            Assert.AreEqual(state, actualState);
            Assert.AreEqual(1, res);
            called = false;
            actual = null;
            actualState = null;

            res = selector.UsingDatabaseResult((r) =>
            {
                called = true;
                actual = r;
                return 1;
            });
            Assert.IsTrue(called);
            Assert.AreEqual(actual, result);
            Assert.AreEqual(1, res);
            called = false;
            actual = null;
        }
    }
}
