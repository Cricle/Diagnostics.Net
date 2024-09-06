using Diagnostics.Traces.Stores;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Test.Stores
{
    [TestClass]
    public class DatabaseAfterSwitchedBaseTest
    {
        [ExcludeFromCodeCoverage]
        class DatabaseCreatedResult : IDatabaseCreatedResult
        {
            public object Root { get; set; }

            public string? FilePath { get; set; }

            public string Key { get; set; }

            public void Dispose()
            {
            }
        }

        [ExcludeFromCodeCoverage]
        class TestDatabaseAfterSwitched<T> : DatabaseAfterSwitchedBase<T>
            where T : IDatabaseCreatedResult
        {
            public TestDatabaseAfterSwitched(IDeleteRules? deleteRules = null, IFileConversionProvider? fileConversionProvider = null):base(deleteRules,fileConversionProvider)
            {

            }
            protected override string FailGetConvertsionPath(string filePath)
            {
                throw new NotImplementedException();
            }

            protected override Stream GetAfterStream(Stream stream)
            {
                throw new NotImplementedException();
            }
        }

        [ExcludeFromCodeCoverage]
        class NullDeleteRules : IDeleteRules
        {
            public void Raise()
            {
            }
        }

        [ExcludeFromCodeCoverage]
        class NullFileConversionProvider : IFileConversionProvider
        {
            public string ConvertPath(string filePath)
            {
                return null;
            }
        }

        [TestInitialize]
        public void Init()
        {
            IOHelper.Init("DatabaseAfterSwitchedBaseTest");
        }

        [TestCleanup]
        public void Cleanup()
        {
            IOHelper.Cleanup("DatabaseAfterSwitchedBaseTest");
        }

        [TestMethod]
        public void PropertiesMustEqualsInputs()
        {
            var rules = new NullDeleteRules();
            var provider = new NullFileConversionProvider();

            var switcher = new TestDatabaseAfterSwitched<IDatabaseCreatedResult>(rules, provider);

            Assert.AreEqual(rules, switcher.DeleteRules);
            Assert.AreEqual(rules, switcher.DeleteRules);
        }
    }
}
