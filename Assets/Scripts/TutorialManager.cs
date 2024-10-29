using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TutorialText; // Reference to tutorial text UI element

    public static TutorialManager Instance { get; private set; }
    public bool InProgress { get; private set; }

    private Dictionary<string, bool> tutorial;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        tutorial = new()
        {
            {"openedInventory", false},
            {"selectedShelf", false},
            {"placedShelf", false},
            {"openedShop", false},
            {"boughtStock", false},
            {"placedStock", false},
            {"soldStock", false}
        };
        InProgress = true;
        AdvanceTutorial();
    }

    // Method to show a specific tutorial step
    public void AdvanceTutorial()
    {
        if (!tutorial["openedInventory"])
        {
            TutorialText.text = "Welcome to your very own supermarket!\n\nTo get started, press [Tab] to open your inventory\n\n(hold [N] to close tutorial";
            return;
        }
        if (!tutorial["selectedShelf"])
        {
            TutorialText.text = "We have provided you with your first shelf!\n\nFind it by going to the Structures section";
            return;
        }
        if (!tutorial["placedShelf"])
        {
            TutorialText.text = "Place your shelf anywhere you want!\n\nYou can rotate using the scroll wheel, or go off-grid with [Ctrl]";
            return;
        }
        if (!tutorial["openedShop"])
        {
            TutorialText.text = "Good job! Now we want to buy stock so we can sell something\n\nPress [Q] to open the shop screen";
            return;
        }
        if (!tutorial["boughtStock"])
        {
            TutorialText.text = "From the Stock section, buy anything you want!\n\nUse the [x10] multiplier to make bulk purchases faster";
            return;
        }
        if (!tutorial["placedStock"])
        {
            TutorialText.text = "Nice!\n\nGrab stock from your inventory [Tab] and place it on your shelf";
            return;
        }
        if (!tutorial["soldStock"])
        {
            TutorialText.text = "Awesome!\n\nNow we wait for customers to come in and buy what's on the shelf";
            return;
        }
        
        InProgress = false;
        HideTutorial();
    }

    // Method to hide the tutorial
    void HideTutorial()
    {
        TutorialText.text = "";
        TutorialText.gameObject.SetActive(false);
    }

    public void CompleteTutorialTask(string task)
    {
        tutorial[task] = true;
        AdvanceTutorial();
    }
}
