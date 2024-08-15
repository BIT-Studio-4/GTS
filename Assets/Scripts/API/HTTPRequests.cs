using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
        if (token.Length > 0) http.SetRequestHeader("Authorization", $"Bearer {token}"); // If a token exists, set it here. This allows the player access to the /api area of our API.

        return await MakeHttpRequest<T>(http);
    }

    public static async Task<T> Post<T>(string url, Dictionary<string, string> data, string token = "")
    {
        // Create a new POST request.
        // Similar to the 'options' parameter of a javascript request.
        UnityWebRequest http = new(url);
        http.method = "POST";
        http.SetRequestHeader("Content-Type", "application/json");

        UploadHandlerRaw uploadHandler = new(Encoding.UTF8.GetBytes(GetJson(data)));
        uploadHandler.contentType = "application/json";
        http.uploadHandler = uploadHandler;
        http.downloadHandler = new DownloadHandlerBuffer();

        if (token.Length > 0) http.SetRequestHeader("Authorization", $"Bearer {token}"); // If a token exists, set it here. This allows the player access to the /api area of our API.

        return await MakeHttpRequest<T>(http);
    }

    // All requests will can use this, may need updating in future.
    // This is the basic error and data handling of the request, and will return the desired type that the relevant request needs.
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

    private static string GetJson(Dictionary<string, string> data)
    {
        StringBuilder sb = new();
        sb.Append("{");

        foreach (KeyValuePair<string, string> kvm in data)
        {
            sb.Append($"\"{kvm.Key}\":\"{kvm.Value}\",");
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");

        return sb.ToString();
    }
}
