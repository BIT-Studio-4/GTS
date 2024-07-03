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
        UnityWebRequest http = UnityWebRequest.Get(url);
        http.method = "GET";
        http.SetRequestHeader("Content-Type", "application/json");

        UnityWebRequestAsyncOperation req = http.SendWebRequest();

        while (!req.isDone) await Task.Yield();

        string response = http.downloadHandler.text;

        if (http.result != UnityWebRequest.Result.Success) Debug.LogError(http.error);

        
    }
}
