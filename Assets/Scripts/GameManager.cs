using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//you can access the singleton instance using Game.Manager.Instance + anything you need (eg GameManager.Instance.AddMoney(amount);
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEvent OnMoneyChange = new UnityEvent();

    [SerializeField] 
    private string apiUrl;

    [SerializeField]
    private string username;

    private User user;
    public User User { 
        get => user; 
        set 
        {
            user = value;
        }
    }

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

            // can add more stuff here, eg updating UI etc later on
            if (user != null) 
            { 
                user.Money = money;
            }
        }
    }

    public UnityEvent onUIOpen;
    public UnityEvent onUIClose;

    void Awake()
    {
        if (Instance != null) //Ensures there is only one instance of GM at once
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(nameof(Initialize));
    }

    private IEnumerator Initialize()
    {
        GetUser();

        yield return new WaitUntil(() => User != null);

        Money = startingMoney;
    }

    private async void GetUser()
    {
        User = await HTTPRequests.Get<User>($"{apiUrl}/users/{username}");
    }
}
