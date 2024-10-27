using System;
using System.Linq;
using System.Threading.Tasks;
using GraphHop.Shared;
using GraphHop.Shared.Data;
using GraphHop.SharedRhino.Data;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
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
        /// Test method
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestMethod]
        public void Test_Add_node()

        {
            var nodeDef = new ComponentDefinitionNode()
            {
                ComponentGuid = Guid.NewGuid(),
                Name = "test"
            };

            _gremlin.Add(nodeDef);
            Assert.IsTrue(_gremlin.Exists(nodeDef));
            var node = _gremlin.Find(nodeDef).ToList();
            Assert.IsTrue(_gremlin.Exists(nodeDef));
            Assert.Equals(node.Count(),2);
        }

        /// <summary>
        /// Test method
        /// Change signature to "async Task" in case of async tests
        /// </summary>
        [TestMethod]
        public void TestBuildVersionGraph()

        {
            var nodeDocument = new DocumentNode()
            {
                DocumentID = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F70"),
            };


            var nodeInstance1 = new ComponentInstanceNode()
            {
                InstanceGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F79"),
                ComponentGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F78"),
                NickName = "1"
            };
            var nodeInstance2 = new ComponentInstanceNode()
            {
                InstanceGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F77"),
                ComponentGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F76"),
                NickName = "2"
            };
            var nodeInstance3 = new ComponentInstanceNode()
            {
                InstanceGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F75"),
                ComponentGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F74"),
                NickName = "2"
            };
            var nodeDef1 = new ComponentDefinitionNode()
            {
                ComponentGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F73"),
                Name = "11"
            };
            var nodeDef2 = new ComponentDefinitionNode()
            {
                ComponentGuid = Guid.Parse("C0357A56-78CF-4C24-834C-794C43EA9F72"),
                Name = "22"
            };
            _gremlin.Add(nodeDocument);
            _gremlin.Add(nodeInstance1);
            _gremlin.Add(nodeInstance2);
            _gremlin.Add(nodeInstance3);

            _gremlin.Add(nodeDef1);
            _gremlin.Add(nodeDef2);

            _gremlin.Connect(nodeDocument, nodeInstance1);
            _gremlin.Connect(nodeDocument, nodeInstance2);
            //_gremlin.Connect(nodeDocument, nodeInstance3);

            _gremlin.Connect(nodeInstance1, nodeDef1);
            _gremlin.Connect(nodeInstance2, nodeDef1);
            _gremlin.Connect(nodeInstance2, nodeDef2);

            var verticesDoc = _gremlin.Find(nodeDocument).ToList();

            var verticesATrav = _gremlin.Find(nodeInstance1);

            var testTraversal = _gremlin.FindConnectedNodes(nodeInstance1).ToList();
            var testTraversal2 = _gremlin.FindConnectedNodes(nodeInstance2).ToList();

            



            var verticesA = _gremlin.Find(nodeInstance1).ToList();
            var verticeB = _gremlin.Find(nodeInstance2).ToList();

            var uniqueToB = verticeB.Except(verticesA);


         //   Assert.IsTrue(_gremlin.Exists(nodeDef));
        }
        [TestMethod]
        public async Task Test_NormalCall()

        {
            var endpoint = "127.0.0.1";
            // This uses the default Neptune and Gremlin port, 8182
            var gremlinServer = new GremlinServer(endpoint, 8182, enableSsl: false);
            var gremlinClient = new GremlinClient(gremlinServer);



           // var gremlinQuery = $"g.V('{12}').repeat(both()).emit().dedup()";
            var gremlinQuery = $"g.V('{12}')";
            var gremlinQuery2 = $"g.V('{12}').out()";
            var gremlinQuery3 = $"g.V()";
            var gremlinQuery4 = $"g.E()";

            var resultSet = await gremlinClient.SubmitAsync<dynamic>(gremlinQuery4);

            var remoteConnection = new DriverRemoteConnection(gremlinClient, "g");
            var gremlin = AnonymousTraversalSource.Traversal().WithRemote(remoteConnection);
            _gremlin = new GremlinGeneric(gremlin);

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