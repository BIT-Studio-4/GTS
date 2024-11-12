using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

//you can access the singleton instance using Game.Manager.Instance + anything you need (eg GameManager.Instance.AddMoney(amount);
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEvent OnMoneyChange = new UnityEvent();

    [SerializeField]
    private string username;
    [SerializeField]
    private string password;
    private string token;
    public string Token { get => token; set => token = value; }

    private User user;
    public User User {
        get => user;
        set
        {
            user = value;
        }
    }

    private SaveGame saveGame;
    public SaveGame SaveGame { get => saveGame; set => saveGame = value; }
    private bool loadGameAttempted = false;

    [SerializeField]
    private int startingMoney = 100; //change to whatever we want

    private int money;
    public int Money
    {
        get => money;
        set
        {
            money = value;
            OnMoneyChange?.Invoke();
        }
    }

    void Awake()
    {
        if (Instance != null) //Ensures there is only one instance of GM at once
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(nameof(Initialize));
    }

    private IEnumerator Initialize()
    {
        LoginUser();

        // TEMPORARY HOTFIX~
        // yield return null;
        yield return new WaitUntil(() => User != null);

        Debug.Log("Logged in");

        GetSaveGame();

        yield return new WaitUntil(() => loadGameAttempted);

        if (saveGame != null)
        {
            LoadManager.Instance.LoadSaveGame(saveGame);
        }
        else
        {
            StartNewGame();
        }
    }

    private async void LoginUser()
    {
        UserLogin login = new()
        {
            name = username,
            password = password,
        };

        // Sends a UserLogin, and returns a User if successful
        User = await HTTPRequests.Post<User, UserLogin>($"{ApiManager.Instance.ApiUrl}/auth/login", login);
        Token = User.token;
    }

    private async void GetSaveGame()
    {
        saveGame = await ApiManager.Instance.LoadSaveGame($"{ApiManager.Instance.ApiUrl}/api/save_games", User);


        loadGameAttempted = true;
    }

    private void StartNewGame()
    {
        Money = startingMoney;
        Debug.Log("Couldn't find a save for this user, starting new game");
    }
}
