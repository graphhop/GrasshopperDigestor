using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphHop.Shared.src.Gremlin;

namespace GraphHop.Tests.Shared
{
    [TestClass]
    public class TestGremlin
    {
        /// <summary>
        /// need to have local server up and running
        /// </summary>
        [TestMethod]
        public void TestGremlinInit()
        {
            var gremlin = new GremlinConncetor();

            gremlin.AddTestObjects();

            var query =  gremlin.GetTestObject();
            Assert.IsNotNull(query);
        }

        /// <summary>
        /// need to have local server up and running
        /// </summary>
        [TestMethod]
        public void TestGremlinAddNode()
        {
            IDictionary<string, object> dummyData = new Dictionary<string, object>();
            dummyData.Add("name", "test1");
            dummyData.Add("guid", new Guid());

            var gremlin = new GremlinConncetor();

            gremlin.AddNode("TestNode", dummyData);

            var query = gremlin.GetNode("TestNode");
            Assert.IsNotNull(query);
        }
    }
}
