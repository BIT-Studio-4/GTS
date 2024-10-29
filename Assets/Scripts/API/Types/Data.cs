using System;

/// <summary>
/// Required when retrieving data from the game's API to destructure the result correctly.
/// </summary>
/// <typeparam name="T">
/// Use the desired 'table' type as this will store the require data in <c>data</c>.
/// </typeparam>

[Serializable]
public class Data<T>
{
    public string msg;
    public T data;
}
