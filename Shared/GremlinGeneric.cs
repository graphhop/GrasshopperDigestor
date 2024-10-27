using System;
using System.Linq;
using System.Reflection;
using GraphHop.Shared.Data;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;

namespace GraphHop.Shared;

/// <summary>
/// Generic interface for Gremlin queries
/// </summary>
public interface IGremlinGeneric
{
    /// <summary>
    /// Add a node.
    /// Only fields with the attributes <see cref="IdAttribute"/>, <see cref="EqualityCheckAttribute"/> 
    /// or <see cref="SerializeAttribute"/> will be added.
    /// </summary>
    /// <param name="node"></param>
    void Add(object node);

    /// <summary>
    /// Find nodes.
    /// This matches nodes by fields with the attributes <see cref="IdAttribute"/> or <see cref="EqualityCheckAttribute"/>.
    /// </summary>
    /// <param name="node">Node to find.</param>
    /// <param name="start">Where to start the graph traversal.</param>
    /// <returns></returns>
    GraphTraversal<T, Vertex> Find<T>(object node, GraphTraversal<T, Vertex> start);

    /// <summary>
    /// Connect nodes by an edge.
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    void Connect(object node1, object node2);

    /// <summary>
    /// Check for existance of a node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool Exists(object node);

    /// <summary>
    /// Check for existance of a connection between two nodes.
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <returns></returns>
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
        return Find(node, _gremlin.V()).HasNext();
    }

    public void Connect(object node1, object node2)
    {
        var n1 = Find(node1, _gremlin.V());
        var n2 = Find(node2, __.V());
        var targetType = node2.GetType();

        if (n1.HasNext() && n1.HasNext())
        {
            n1.AddE(targetType.Name).To(n2).Next();
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
        var n1 = Find(node1, _gremlin.V());
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

    public GraphTraversal<T, Vertex> Find<T>(object node, GraphTraversal<T, Vertex> start)
    {
        var nodeType = node.GetType();
        var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var idAttributeType = typeof(IdAttribute);
        var idField = fieldInfos.Where(field => field.IsDefined(idAttributeType, false)).FirstOrDefault();
        if (idField == null)
        {
            throw new Exception("No IdAttribute found on node");
        }
        var equalityAttributeType = typeof(IdAttribute);
        var equalityFields = fieldInfos.Where(field => field.IsDefined(idAttributeType, false)).ToList();
    
        // get node by label and id
        var traversal = start
            .HasLabel(nodeType.Name)
            .Has(idField.Name, idField.GetValue(node));

        // filter by equality fields
        foreach (var field in equalityFields)
        {
            traversal = traversal.Has(field.Name, field.GetValue(node));
        }

        return traversal;
    }

    public void Add(object node)
    {
        var nodeType = node.GetType();
        var x = _gremlin.AddV(nodeType.Name);

        var fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(field => field.IsDefined(typeof(IdAttribute), false)
                            | field.IsDefined(typeof(EqualityCheckAttribute), false)
                            | field.IsDefined(typeof(SerializeAttribute), false)
            );

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