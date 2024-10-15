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

        Test test = new()
        {
            name = "hi",
            age = 5,
            money = 10.5,
            Children = new[] { "hello", "hey", "hiya" },
            User = new User { name = "test-user", id = "0", Money = 200, token = "example-token"},
        };

        Debug.Log(HTTPRequests.GetJson(test));
    }

    public async void UpdateUser(string url, User user)
    {
        await HTTPRequests.Put<User, User>(url, user, user.token);
    }
}
