﻿using System.Collections.Generic;
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

        // Call this everytime we begin adding or reading
        public GraphTraversalSource BeginTransaction(GraphTraversalSource g)
        {
          return g.Tx().Begin();
        }

        // Call this every time we finish a transaction
        public async void CommitTransaction(GraphTraversalSource g, GraphTraversalSource gtx)
        {
            await gtx.Tx().CommitAsync();
        }


        public void AddTestObjects() 
        {      
            var tx = BeginTransaction(_gremlin);
                var v1 = tx.AddV("person").Property("name", "marko").Next();
                var v2 = tx.AddV("person").Property("name", "stephen").Next();
            CommitTransaction(_gremlin, tx);
        }
        public IEnumerable<Vertex> GetTestObject()
        {
            var marko = _gremlin.V().Has("person", "name", "marko").Next();
            return _gremlin.V().Has("person", "name", "marko").Out("knows").ToList();
        }

        public void AddNode(string label, IDictionary<string, object> properties)
        {
            
            var v1 = _gremlin.AddV(label).Property("name", "marko").Next();

        }


    }
 }

