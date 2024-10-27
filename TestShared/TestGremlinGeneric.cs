using System;
using GraphHop.Shared;
using GraphHop.Shared.Data;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginTemplate.Tests.Shared
{
    [TestClass]
    public class TestGremlinGeneric
    {
        static IGremlinGeneric _gremlin;

        /// <summary>
        /// Test setup for complete class, will be called once for all tests contained herein
        /// Change signature to "async static Task" in case of async tests
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var endpoint = "127.0.0.1";
            // This uses the default Neptune and Gremlin port, 8182
            var gremlinServer = new GremlinServer(endpoint, 8182, enableSsl: false);
            var gremlinClient = new GremlinClient(gremlinServer);
            var remoteConnection = new DriverRemoteConnection(gremlinClient, "g");
            var gremlin = AnonymousTraversalSource.Traversal().WithRemote(remoteConnection);
            _gremlin = new GremlinGeneric(gremlin);
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
        public void Test_Add_Exists_00()
        {
            var node = new ComponentDefinitionNode()
            {
                ComponentGuid = Guid.NewGuid(),
                Name = "test"
            };
            _gremlin.Add(node);
            Assert.IsTrue(_gremlin.Exists(node)); ;
        }

        /// <summary>
        /// Test method
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestMethod]
        public void Test_Add_Exists_01()
        {
            var node = new ComponentInstanceNode()
            {
                ComponentGuid = Guid.NewGuid(),
                NickName = "test"
            };
            _gremlin.Add(node);
            Assert.IsTrue(_gremlin.Exists(node)); ;
        }

        /// <summary>
        /// Test method
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestMethod]
        public void Test_Connect_Exists_00()
        {
            var nodeDef = new ComponentDefinitionNode()
            {
                ComponentGuid = Guid.NewGuid(),
                Name = "test"
            };
            var nodeInstance = new ComponentInstanceNode()
            {
                InstanceGuid = Guid.NewGuid(),
                NickName = "test"
            };
            _gremlin.Add(nodeDef);
            _gremlin.Add(nodeInstance);
            Assert.IsTrue(_gremlin.Exists(nodeDef));
            Assert.IsTrue(_gremlin.Exists(nodeInstance));
            _gremlin.Connect(nodeDef, nodeInstance);
            Assert.IsTrue(_gremlin.ConnectionExists(nodeDef, nodeInstance));
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