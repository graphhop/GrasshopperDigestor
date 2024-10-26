using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Rhino.Geometry;
using GraphHop.SharedRhino;
using Grasshopper.Kernel;

namespace GraphHop.Tests.SharedRhino
{
    [TestClass]
    public class TestGHIngest
    {
        GH_Document OpenGrasshopperDocument(string filename)
        {
            // Load the Grasshopper document
            var io = new GH_DocumentIO();
            if (!io.Open(filename))
            {
                return null;
            }

            return io.Document;
        }
        
        /// <summary>
        /// Test setup for complete class, will be called once for all tests contained herein
        /// Change signature to "async static Task" in case of async tests
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // TODO 
        }

        /// <summary>
        /// Test setup per test, will be called once for each test
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // TODO
        }

        /// <summary>
        /// Test method
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestMethod]
        public void TestIngest()
        {
            var testDoc = OpenGrasshopperDocument()
            GraphHop.SharedRhino.Utilities.GHDigestUtility
        }

        /// <summary>
        /// Test cleanup per test, will be called once for each test
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // TODO 
        }

        /// <summary>
        /// Test cleanup for complete class, will be called once for all tests contained herein
        /// Change signature to "async static Task" in case of async tests
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // TODO 
        }
    }
}