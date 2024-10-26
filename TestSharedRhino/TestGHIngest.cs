using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Rhino.Geometry;
//using PluginTemplate.SharedRhino;
using GraphHop.SharedRhino;
using Grasshopper.Kernel;

namespace GraphHop.Tests.SharedRhino
{
    [TestClass]
    public class TestGHIngest
    {
        public static string GetPathRelativeToAssembly(string filename)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(directory ?? string.Empty, $"../../../Sample File/{filename}");
        }
        
        GH_Document OpenGrasshopperDocument(string filename, out GH_DocumentIO io)
        {
            var fullFilePath = GetPathRelativeToAssembly(filename);
            // Load the Grasshopper document
            io = new GH_DocumentIO();
            if (!io.Open(fullFilePath))
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
          //  var sharedRhinoExample = new SharedRhinoExample();
         //   var point = sharedRhinoExample.PlaneLineIntersection(Plane.WorldXY, new Line(new Point3d(1,1,-1), new Point3d(1,1,1)));
         //   Assert.AreEqual(0, point.DistanceToSquared(new Point3d(1,1,0)));
            var testFile = "Sample Grasshopper File/grasshopper-examples-master/gh/amoeba-curve-2d.ghx";
            var testDoc = OpenGrasshopperDocument(testFile, out var io);
            Assert.IsNotNull(testDoc);
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