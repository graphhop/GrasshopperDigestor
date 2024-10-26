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
        void Add(object node);

        GraphTraversal<Vertex, Vertex> Find(object node);

        GraphTraversal<object, Vertex> FindAnonymous(object node);


        void Connect(object node1, object node2);

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

        public void Connect(object node1, object node2)
        {
            var n1 = Find(node1);
            var n2 = FindAnonymous(node2);
            var targetType = node2.GetType();

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
        }

        public bool ConnectionExists(object node1, object node2)
        {
            throw new NotImplementedException();
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

        public GraphTraversal<Vertex, Vertex> Find(object node)
        {
            var nodeType = node.GetType();
            var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attType = typeof(IdAttribute);
            var idField = fieldInfos.Where(field => field.IsDefined(attType, false)).FirstOrDefault();
            if (idField == null)
            {
                throw new Exception("No IdAttribute found on node");
            }

            return _gremlin.V()
                .HasLabel(nodeType.Name)
                .Has(idField.Name, idField.GetValue(node));
        }

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

    }
 }

