using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class HTTPRequests<T>
{
    public static async Task<T> Get(string url)
    {
        // Create a new GET request.
        // Similar to the 'options' parameter of a javascript request.
        UnityWebRequest http = UnityWebRequest.Get(url);
        http.method = "GET";
        http.SetRequestHeader("Content-Type", "application/json");

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
            T result = JsonUtility.FromJson<T>(response); // Converts the JSON to the 'T' type provided whenever the method is called.

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
