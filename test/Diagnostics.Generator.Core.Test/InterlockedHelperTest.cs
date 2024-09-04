namespace Diagnostics.Generator.Core.Test
{
    [TestClass]
    public class InterlockedHelperTest
    {
        private async Task BatchExecute(int taskCount,Action doing)
        {
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(doing);
            }
            await Task.WhenAll(tasks);
        }

        [TestMethod]
        public async Task AddDouble()
        {
            double value = 0;
            await BatchExecute(100, () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    InterlockedHelper.Add(ref value, 1);
                }
            });

            Assert.AreEqual(value, 100 * 100d);
        }
        [TestMethod]
        public async Task AddFloat()
        {
            float value = 0;
            await BatchExecute(100, () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    InterlockedHelper.Add(ref value, 1);
                }
            });

            Assert.AreEqual(value, 100 * 100f);
        }
    }
}
