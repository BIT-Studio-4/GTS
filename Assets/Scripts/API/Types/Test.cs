using System;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class Test
{
    public string name;
    public int age;
    public double money;
    public string[] children;
    public bool verified;
    public User user;
}
