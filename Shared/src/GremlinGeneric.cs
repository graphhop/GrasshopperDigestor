using Gremlin.Net.Process.Traversal;
using System.Reflection;
using GraphHop.Shared.Data;
using Gremlin.Net.Structure;
using System.Linq;
using System;

namespace GraphHop.Shared
{
    
    public interface IGremlinGeneric
    {
        void Add(object node, bool addNewOnly = true);

        GraphTraversal<Vertex, Vertex> Find(object node, GraphTraversal<Vertex, Vertex> start = null);

        GraphTraversal<object, Vertex> FindAnonymous(object node);
        GraphTraversal<Vertex, Vertex> FindConnectedNodes(object node, int depth = 10);

        bool Connect(object node1, object node2);

        bool Exists(object node);

        bool ConnectionExists(object node1, object node2);
    }

    public class GremlinGeneric : IGremlinGeneric
    {
        GraphTraversalSource _gremlin;
        public GremlinGeneric(GraphTraversalSource gremlin)
        {
            _gremlin = gremlin;
            
        }

        public bool Exists(object node)
        {
            return Find(node).HasNext();
        }

        public bool Connect(object node1, object node2)
        {
            /*
            var n1 = Find(node1);
            var n2 = FindAnonymous(node2);
            var targetType = node2.GetType();
            */
            var isThere = ConnectionExists(node1, node2);


            var n1 = Find(node1);
            if (!n1.HasNext())
            {
                return false;
            }
            var targetType = node2.GetType();
            var result = n1.OutE(targetType.Name).InV();
            if (!result.HasNext())
            {
                return false;
            }
            return Find(node2, result).HasNext();

            /*
            if (n1.HasNext() && n1.HasNext())
            {
                n1.AddE(targetType.Name).To(n2);
            }
            else if (!n1.HasNext())
            {
                throw new Exception("Source node not found");
            }
            else if (!n2.HasNext())
            {
                throw new Exception("Target node not found");
            }
            */
            var isThereAfterAdd = ConnectionExists(node1, node2);

            var isThereAfterAddReverse = ConnectionExists(node2, node1);

            /*
            if (n1 is null)
            {
                throw new Exception("Source node not found");
            }
            if (n2 is null)
            {
                throw new Exception("Target node not found");
            }
            n1.AddE(targetType.Name).To(n2).Next();         
            */

        }

        public bool ConnectionExists(object node1, object node2)
        {
            var n1 = Find(node1);
            if (!n1.HasNext())
            {
                return false;
            }
            var targetType = node2.GetType();
            var result = n1.OutE(targetType.Name).InV();
            if (!result.HasNext())
            {
                return false;
            }
            return Find(node2, result).HasNext();
        }

        public GraphTraversal<object, Vertex> FindAnonymous(object node)
        {
            var nodeType = node.GetType();
            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attType = typeof(IdAttribute);
            var idField = fieldInfos.Where(field => field.IsDefined(attType, false)).FirstOrDefault();
            if (idField == null)
            {
                throw new Exception("No IdAttribute found on node");
            }

            return __.V()
                .HasLabel(nodeType.Name)
                .Has(idField.Name, idField.GetValue(node));
        }

        public GraphTraversal<Vertex, Vertex> Find(object node, GraphTraversal<Vertex, Vertex> start = null)
        {
            var nodeType = node.GetType();
            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attType = typeof(IdAttribute);
            var idField = fieldInfos.Where(field => field.IsDefined(attType, false)).FirstOrDefault();
            if (idField == null)
            {
                throw new Exception("No IdAttribute found on node");
            }

            return (start != null ? start : _gremlin.V())
                .HasLabel(nodeType.Name)
                .Has(idField.Name, idField.GetValue(node));
        }

        public GraphTraversal<Vertex, Vertex> FindConnectedNodes(object node, int depth = 10)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var nodeType = node.GetType();
            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attType = typeof(IdAttribute);
            var idField = fieldInfos.FirstOrDefault(field => field.IsDefined(attType, false));

            if (idField == null)
            {
                throw new InvalidOperationException("No IdAttribute found on node");
            }

            var nodeId = idField.GetValue(node);
            if (nodeId == null)
            {
                throw new InvalidOperationException("Node ID cannot be null");
            }


            var test = _gremlin.V();
            var test2 = _gremlin.V().ToList();

            var test22 = _gremlin.E().ToList();


            var testHas2 = _gremlin.V().HasLabel(nodeType.Name).Has(idField.Name, nodeId).Out().ToList();
            _gremlin.V().In();

            var test45 = _gremlin.V().Repeat(new GraphTraversal<Vertex, Vertex>().Both()).Out().Path().ToList();


            var test455 = _gremlin.V().HasLabel(nodeType.Name).Repeat(new GraphTraversal<Vertex, Vertex>().Both()).Out().Path().ToList();


            /*
            g.V().has("name", "gremlin").
              repeat(in ("manages")).
                until(has("title", "ceo")).
              path().by("name")
            */

            foreach (var vertex in _gremlin.V().HasLabel(nodeType.Name).ToList())
            {
                var db = vertex.Id;

                // var trest = _gremlin.V().HasLabel(vertex.Label).Both().ToList();
                Console.WriteLine($"{vertex.Id}");
            }

            /*
            g.V().has('code', 'AUS').
      repeat(out ().simplePath()).
        until(has('code', 'AGR')).
        path().by('code').limit(10)
            */


            var testHas = _gremlin.V().HasLabel(nodeType.Name).Has(idField.Name, nodeId).OutE();
              //  .Has(idField.Name, nodeId).OutV().ToList();

            /*
            var traversal = (_gremlin.V().OutV)
                .HasLabel(nodeType.Name)
                .Has(idField.Name, nodeId)
                .Repeat(__.Both().SimplePath()).Times(depth)
                .Emit();
            */
            //

          //  var test3 = traversal.ToList();

            return test;
        }




        /*
        public void Add(object node)
        {


            var nodeType = node.GetType();
            var x = _gremlin.AddV(nodeType.Name);

            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
         
            // loop over all properties of the node and add them to the vertex
            foreach (var field in fieldInfos)
            {
                var value = field.GetValue(node);
                if (value != null)
                {
                    x = x.Property(field.Name, value);
                }
            }

            x.Next();
        }
        */

        public void Add(object node, bool addNewOnly = true)
        {
            if (addNewOnly)
            {
                var getNode = Find(node).ToList();
                if (getNode.Count != 0) return;
            }

            var nodeType = node.GetType();
            var x = _gremlin.AddV(nodeType.Name);

            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // loop over all properties of the node and add them to the vertex
            foreach (var field in fieldInfos)
            {
                var value = field.GetValue(node);
                if (value != null)
                {
                    x = x.Property(field.Name, value);
                }
            }

            x.Next();
        }

    }
 }
