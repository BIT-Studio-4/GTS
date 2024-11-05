using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TutorialText; // Reference to tutorial text UI element
    [SerializeField] private CanvasGroup canvasGroup;

    public static TutorialManager Instance { get; private set; }

    private Dictionary<string, bool> tutorial;
    public Dictionary<string, bool> Tutorial { get => tutorial; }

    private bool inProgress;
    public bool InProgress { get => inProgress; }

    private Coroutine fade;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        InputSystem.actions.FindAction("SkipTutorial").performed += SkipTutorial;
    }

    public void Start()
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

        if (fade != null) StopCoroutine(fade);
        inProgress = true;

        AdvanceTutorial();
        fade = StartCoroutine(FadeIn());
    }

    // Method to show a specific tutorial step
    public void AdvanceTutorial()
    {
        if (!tutorial["openedInventory"])
        {
            TutorialText.text = "Welcome to your very own supermarket!\nTo get started, press [Tab] to open your inventory\n(hold [N] to close tutorial)";
            return;
        }
        if (!tutorial["selectedShelf"])
        {
            TutorialText.text = "We have provided you with your first shelf!\nFind it by going to the Structures section";
            return;
        }
        if (!tutorial["placedShelf"])
        {
            TutorialText.text = "Place your shelf anywhere you want!\nYou can rotate using the scroll wheel, or go off-grid with [Ctrl]";
            return;
        }
        if (!tutorial["openedShop"])
        {
            TutorialText.text = "Good job! Now we want to buy stock so we can sell something\nPress [Q] to open the shop screen";
            return;
        }
        if (!tutorial["boughtStock"])
        {
            TutorialText.text = "From the Stock section, buy anything you want!\nUse the [x10] multiplier to make bulk purchases faster";
            return;
        }
        if (!tutorial["placedStock"])
        {
            TutorialText.text = "Nice!\nGrab stock from your inventory [Tab] and place it on your shelf";
            return;
        }
        if (!tutorial["soldStock"])
        {
            TutorialText.text = "Awesome!\nNow we wait for customers to come in and buy what's on the shelf";
            return;
        }

        StartCoroutine(EndTutorial());
    }

    public void CompleteTutorialTask(string task)
    {
        tutorial[task] = true;

        // edge case - skip telling user to open shop if they already bought something
        if (task == "placedShelf" && !tutorial["boughtStock"])
        {
            tutorial["openedShop"] = false;
        }

        AdvanceTutorial();
    }

    void SkipTutorial(InputAction.CallbackContext ctx)
    {
        inProgress = false;
        string[] tasks = tutorial.Keys.ToArray();
        foreach (string task in tasks)
        {
            tutorial[task] = true;
        }
        TutorialText.text = "Skipping tutorial...";
        fade = StartCoroutine(FadeOut());
    }

    IEnumerator EndTutorial()
    {
        inProgress = false;
        TutorialText.text = "That is the end of the tutorial!\nGo and make some profit!!";
        yield return new WaitForSeconds(3);
        fade = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 0.5f;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += Time.deltaTime * 0.5f;
            yield return null;
        }
    }
}
