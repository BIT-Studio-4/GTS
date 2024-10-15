using System;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class User
{
    public string id;
    public string name;
    private int money;
    public int Money
    {
        get => money;
        set
        {
            money = value;

            ApiManager.Instance.UpdateUser($"{ApiManager.Instance.ApiUrl}/api/users/{id}", this);
        }
    }
    public string token;
}
