using System;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class Store
{
    public string id;
    public StoreObject[] store_objects;
}
