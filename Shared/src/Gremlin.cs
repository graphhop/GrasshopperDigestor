using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;

namespace GraphHop.Shared.src.Gremlin
{
    /// <summary>
    /// Conncets to Thinkerpop throu Gremlin
    /// </summary>
    public class GremlinConncetor
    {
        GraphTraversalSource _gremlin;
        public GremlinConncetor()
        {
            var gremlinServer = new GremlinServer("localhost", 8182);
            var gremlinClient = new GremlinClient(gremlinServer);
            var driverRemoteConnection = new DriverRemoteConnection(gremlinClient, "g");
            _gremlin = AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);

            // _gremlin = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection("localhost", 8182));
        }

        public void AddTestObjects()
        {
            var v1 = _gremlin.AddV("person").Property("name", "marko").Next();
            var v2 = _gremlin.AddV("person").Property("name", "stephen").Next();
        }
        public IEnumerable<Vertex> GetTestObject()
        {
            var marko = _gremlin.V().Has("person", "name", "marko").Next();
            return _gremlin.V().Has("person", "name", "marko").Out("knows").ToList();
        }

        public Vertex GetNode(string label)
        {
            var node = _gremlin.V().Has(label).ToList();
            return node?.First();
        }

        public void AddNode(string label, IDictionary<string, object> properties)
        {

            var vertex = _gremlin.AddV(label).Property("name", "testName").Next();

            foreach (var property in properties)
            {
                _gremlin.V().Property(property.Key, property.Value);
            }


        }
    }
}
