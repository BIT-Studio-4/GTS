using System;

/// <summary>
/// Allows user login info to be sent correctly to API.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class UserLogin
{
    public string name;
    public string password;
}
