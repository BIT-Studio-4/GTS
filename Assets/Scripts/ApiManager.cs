using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [SerializeField]
    private string apiUrl;
    public string ApiUrl { get => apiUrl; set => apiUrl = value; }

    private void Awake()
    {
        Instance = this;
    }

    public async void UpdateUser(string url, User user)
    {
        Debug.Log("hi");
        await HTTPRequests.Put<User, User>(url, user, user.token);
        Debug.Log("bye");
    }
}
