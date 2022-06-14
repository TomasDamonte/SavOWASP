using Microsoft.VisualStudio.TestTools.UnitTesting;
using SavOWASP;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        private ScannerController Controller = new ScannerController();
        private string TestURL = "https://conectate.ub.edu.ar";

        [TestMethod]
        public void TestScan()
        {
            var result = Controller.Scan(TestURL);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestScanToHtml()
        {
            var result = Controller.ScanToHtml(TestURL);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);            
        }

    }
}
