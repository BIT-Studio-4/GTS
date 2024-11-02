using System.Threading.Tasks;
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

    public async void CreateSaveGame(string url, SaveGame saveGame, User user)
    {
        await HTTPRequests.Post<SaveGame, SaveGame>(url, saveGame, user.token);
    }

    public async Task<SaveGame> LoadSaveGame(string url, User user)
    {
        return await HTTPRequests.Get<SaveGame>($"{url}/{user.id}", user.token);
    }
}
