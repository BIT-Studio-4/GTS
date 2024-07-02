using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//you can access the singleton instance using Game.Manager.Instance + anything you need (eg GameManager.Instance.AddMoney(amount);
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    void Awake() //only one instance of GM at once
    {
        if (Instance == null)
        { Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        Money = startingMoney;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void SubtractMoney(int amount)
    {
        Money -= amount;
    }    
}
