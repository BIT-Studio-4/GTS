using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles all HTTP requests that the game requires to function.
/// </summary>

public class HTTPRequests
{
    public static async Task<T> Get<T>(string url, string token = "")
    {
        // Create a new GET request.
        // Similar to the 'options' parameter of a javascript request.
        UnityWebRequest http = UnityWebRequest.Get(url);
        http.method = "GET";
        http.SetRequestHeader("Content-Type", "application/json");
        if (token.Length > 0) http.SetRequestHeader("Authorization", $"Bearer {token}");

        return await MakeHttpRequest<T>(http);
    }

    public static async Task<T> Post<T>(string url, WWWForm body, string token = "")
    {
        // Create a new GET request.
        // Similar to the 'options' parameter of a javascript request.
        UnityWebRequest http = UnityWebRequest.Post(url, body);
        http.method = "POST";
        http.SetRequestHeader("Content-Type", "application/json");
        if (token.Length > 0) http.SetRequestHeader("Authorization", $"Bearer {token}");

        return await MakeHttpRequest<T>(http);
    }

    private static async Task<T> MakeHttpRequest<T>(UnityWebRequest http)
    {
        // Make the request to the API.
        // Similar to running 'fetch(url, options)'.
        UnityWebRequestAsyncOperation req = http.SendWebRequest();

        // Wait until request is finished.
        // 'Task.Yield()' waits until the condition is true to continue without slowing down other processes.
        while (!req.isDone) await Task.Yield();

        string response = http.downloadHandler.text;

        if (http.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(http.error);

            return default;
        }

        // Attempt to parse the JSON result to an Object that represents the data.
        try
        {
            // Due to the way our API formats responses, the extra 'data' step is necessary.
            Data<T> data = JsonUtility.FromJson<Data<T>>(response); // Converts the JSON to the 'T' type provided whenever the method is called.
            T result = data.data;

            Debug.Log(http.downloadHandler.text);

            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);

            return default; // Essentially returning null but ensuring that non-nullable types still work.
        }
    }
}
