using Diagnostics.Traces.Stores;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class DayOrLimitDatabaseSelectorTest
    {
        [ExcludeFromCodeCoverage]
        class DatabaseCreatedResult : IDatabaseCreatedResult
        {
            public object Root { get; set; }

            public string? FilePath { get; set; }

            public string Key { get; set; }

            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [ExcludeFromCodeCoverage]
        class FlushedDatabaseCreatedResult : IDatabaseCreatedResult
        {
            public object Root { get; set; }

            public string? FilePath { get; set; }

            public string Key { get; set; }

            public static readonly List<FlushedDatabaseCreatedResult> Instances = new List<FlushedDatabaseCreatedResult>();

            public int Id;

            public bool IsDisposed;

            public int HitCount;

            public FlushedDatabaseCreatedResult()
            {
                Id = Instances.Count;
                Instances.Add(this);
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [TestMethod]
        public void GivenNull_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void GivenFailLimitCount_MustThrowArgumentException(int count)
        {
            Assert.ThrowsException<ArgumentException>(() => new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new DatabaseCreatedResult(), count));
        }

        [TestMethod]
        public void Flush_MustSwitched()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 100);
            selector.Flush();

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.IsFalse(FlushedDatabaseCreatedResult.Instances[0].IsDisposed);

            selector.Flush();
            Assert.AreEqual(2, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.IsTrue(FlushedDatabaseCreatedResult.Instances[0].IsDisposed);
            Assert.IsFalse(FlushedDatabaseCreatedResult.Instances[1].IsDisposed);
        }

        [TestMethod]
        public void DangerousGetResult_MustGotResult()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 100);
            Assert.IsNull(selector.DangerousGetResult());
            selector.Flush();

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(FlushedDatabaseCreatedResult.Instances[0], selector.DangerousGetResult());
        }

        [TestMethod]
        public void ReportInserted_OutRangeMustSwitched()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 1);
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.IsFalse(FlushedDatabaseCreatedResult.Instances[0].IsDisposed);
        }

        [TestMethod]
        public void ReportInserted_NoOutRangeMustSwitched()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.ReportInserted(1);

            Assert.AreEqual(0, FlushedDatabaseCreatedResult.Instances.Count);
        }

        [TestMethod]
        public void UsingDatabaseResultDelegate_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.UsingDatabaseResult(r => r.HitCount++);
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
        }

        [TestMethod]
        public void UsingDatabaseResult_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.UsingDatabaseResult(r =>
            {
                r.HitCount++;
            });
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
        }

        [TestMethod]
        public void UsingDatabaseResultWithResult_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            var res = selector.UsingDatabaseResult(r =>
            {
                return ++r.HitCount;
            });
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void UsingDatabaseResultWithState_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            var obj = new object();
            object o = null!;
            selector.UsingDatabaseResult<object>(obj, (res, state) =>
            {
                Assert.AreEqual(FlushedDatabaseCreatedResult.Instances[0], res);
                o = state;
            });

            Assert.AreEqual(obj, o);
        }

        [TestMethod]
        public void AddSwitched_SwitchMustBeCall()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);

            var called = false;

            selector.AfterSwitcheds.Add(new DelegateAfterSwitched<FlushedDatabaseCreatedResult>(res =>
            {
                called = true;
            }));

            selector.Flush();
            selector.Flush();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Initialized_SwitchMustBeCall()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);

            var called = false;

            selector.Initializers.Add(new DelegateResultInitializer<FlushedDatabaseCreatedResult>(res =>
            {
                called = true;
            }));

            selector.Flush();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void InitializedAndAddSwitched_SwitchMustBeCall()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 1);

            var calledInit = false;
            var calledSwitched = false;

            selector.Initializers.Add(new DelegateResultInitializer<FlushedDatabaseCreatedResult>(res =>
            {
                calledInit = true;
            }));
            selector.AfterSwitcheds.Add(new DelegateAfterSwitched<FlushedDatabaseCreatedResult>(res =>
            {
                calledSwitched = true;
            }));

            Assert.IsTrue(selector.Flush());

            Assert.IsTrue(calledInit);
            Assert.IsFalse(calledSwitched);

            selector.Flush();
            Assert.IsTrue(calledSwitched);
        }

        [TestMethod]
        public void Disposed_TheResultWillBeDisposed()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 100);
            selector.Flush();
            selector.Dispose();

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.IsTrue(FlushedDatabaseCreatedResult.Instances[0].IsDisposed);
        }

        [TestMethod]
        public void DatabaseCreatorMustEqualsInput()
        {
            Func<FlushedDatabaseCreatedResult> res = () => new FlushedDatabaseCreatedResult();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(res, 100);

            Assert.AreEqual(res, selector.DatabaseCreator);
        }

        [TestMethod]
        public void DisposeMoreIsOk()
        {
            var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 100);
            selector.Dispose();
            selector.Dispose();
        }

        [TestMethod]
        public void UnsafeUsingDatabaseResultWithResult_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            var res = selector.UnsafeUsingDatabaseResult(r =>
            {
                return ++r.HitCount;
            });
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void UnsafeReportInserted_NoOutRangeMustSwitched()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<IDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.UnsafeReportInserted(1);

            Assert.AreEqual(0, FlushedDatabaseCreatedResult.Instances.Count);
        }

        [TestMethod]
        public void UnsafeUsingDatabaseResultDelegate_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.UnsafeUsingDatabaseResult(r => r.HitCount++);
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
        }

        [TestMethod]
        public void UnsafeUsingDatabaseResult_MoustGotThat()
        {
            FlushedDatabaseCreatedResult.Instances.Clear();

            using var selector = new DayOrLimitDatabaseSelector<FlushedDatabaseCreatedResult>(() => new FlushedDatabaseCreatedResult(), 2);
            selector.UnsafeUsingDatabaseResult(r =>
            {
                r.HitCount++;
            });
            selector.ReportInserted(1);

            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances.Count);
            Assert.AreEqual(1, FlushedDatabaseCreatedResult.Instances[0].HitCount);
        }
    }
}
