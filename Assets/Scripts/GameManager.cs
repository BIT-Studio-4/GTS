using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//you can access the singleton instance using Game.Manager.Instance + anything you need (eg GameManager.Instance.AddMoney(amount);
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] 
    private string apiUrl;

    [SerializeField]
    private string username;

    private User user;
    public User  User { get => user; set => user = value; }

    [SerializeField]
    private int startingMoney = 100; //change to whatever we want

    private int money;
    public int Money
    {
        get => money;
        private set
        {
            money = value;
            // can add more stuff here, eg updating UI etc later on
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
        Initialize();
    }

    private void Initialize()
    {
        GetUser();
        Money = startingMoney;
    }

    private async void GetUser()
    {
        User = await HTTPRequests.Get<User>($"{apiUrl}/users/{username}");
    }
}
