using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class DelegatePhysicalPathProviderTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegatePhysicalPathProvider<int>(null));
        }

        [TestMethod]
        public void GetPath()
        {
            Func<int, string> getter = a => a.ToString();

            var provider = new DelegatePhysicalPathProvider<int>(getter);

            Assert.AreEqual(provider.Getter, getter);
            Assert.AreEqual(provider.GetPath(1), "1");
        }
    }
}
