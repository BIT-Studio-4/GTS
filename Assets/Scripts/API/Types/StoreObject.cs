using System;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class StoreObject
{
    public int item_id;
    public string store_id;
    public float x_pos;
    public float y_pos;
    public float z_pos;
    public float y_rot;
}
