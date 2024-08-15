using System;
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

            Debug.Log($"{name}: {money:c}");

            // Update the API with the new money value;
        }
    }
    private string token;
    public string Token
    {
        get => token;
        set
        {
            token = value;

            Debug.Log($"{token:c}");

            // Update the API with the new money value;
        }
    }
}
