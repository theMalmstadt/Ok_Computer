using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OBM.Controllers;

namespace OBMTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fuck = 69;

            var fucker = HomeController.TestBool(fuck);

            Assert.IsFalse(fucker);
        }
    }
}
