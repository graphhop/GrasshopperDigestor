using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;

namespace GraphHop.Shared.src.Gremlin
{
    /// <summary>
    /// Conncets to Thinkerpop throu Gremlin
    /// </summary>
    public class Gremlin
    {
        GraphTraversalSource _gremlin;
        public Gremlin()
        {
            //var gremlinServer = new GremlinServer("localhost", 8182);
            //using var gremlinClient = new GremlinClient(gremlinServer);
            _gremlin = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection("localhost", 8182));
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
    }
 }

