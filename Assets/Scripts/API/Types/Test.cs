using System;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class on creation.
public class Test
{
    public string name;
    public int age;
    public double money;
    private string[] children;
    public string[] Children { get => children; set => children = value; }
    public bool verified;
    private User user;
    public User User { get => user;  set => user = value; }
}
