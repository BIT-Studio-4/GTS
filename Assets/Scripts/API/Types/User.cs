using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows user info from a JSON response to be stored in a type safe and specific instance.
/// </summary>

[Serializable] // Means that data can be parsed and stored within an instance of this class of creation.
public class User
{
    public string id;
    public string name;
    private double money;
    public double Money
    {
        get => money;
        set
        {
            money = value;

            //Dictionary<string, string> data = new()
            //{
            //    { "money", money.ToString() },
            //};
            //HTTPRequests.Put<User>($"{GameManager.Instance.ApiUrl}/api/users/{id}", data, token);
            // Update the API with the new money value;
        }
    }
    public string token;
}
