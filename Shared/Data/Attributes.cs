using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// Attribute for defining the field that represents the ID of a node.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class IdAttribute : Attribute
{
   
}

/// <summary>
/// Attribute for defining the fields that should be used for equality checking.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class EqualityCheckAttribute : Attribute
{

}

/// <summary>
/// Attribute for defining the fields that should be serialized to the database.
/// This does not need to be added if <see cref="IdAttribute"/> or <see cref="EqualityCheckAttribute"/> is added.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class SerializeAttribute : Attribute
{

}