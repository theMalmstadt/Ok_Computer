using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OBM.Controllers;
using System.Diagnostics;
namespace OBMTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void TestMethod2()
        {
            //te
            EventsController myController = new EventsController();

            var result = myController.ResponsiveEvents("asdf");
            Debug.WriteLine(result);

            Assert.IsNotNull(result);
        }
    }
}
