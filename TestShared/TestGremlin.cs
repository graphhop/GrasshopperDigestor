using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphHop.Shared;

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
            var gremlin = new GremlinConnector();

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

            var gremlin = new GremlinConnector();

            gremlin.AddNode("TestNode2", dummyData);

            var query = gremlin.GetNode("TestNode2");
            Assert.IsNotNull(query);
        }
    }
}
