using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TutorialText; // Reference to tutorial text UI element

    public static TutorialManager Instance { get; private set; }

    private Dictionary<string, bool> tutorial = new();
    public Dictionary<string, bool> Tutorial { get => tutorial; }

    private int currentTutorialStep = 0; // Tracks current tutorial step

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
        StartTutorial();
    }

    private void Update()
    {
        // Check if the "N" key is pressed and advances tutorial step
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            NextTutorialStep();
        }
        else if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.right.wasPressedThisFrame) NextTutorialStep();
        }
    }


    // Method to start the tutorial
    public void StartTutorial()
    {
        ShowTutorialStep(0); // Starts with the first step
    }

    // Method to show a specific tutorial step
    public void ShowTutorialStep(int stepIndex)
    {
        TutorialText.text = "";

        switch (stepIndex)
        {
            case 0:
                TutorialText.text = "Welcome to your very own supermarket!\n\nThis is the tutorial, which will help you to get started.\n\n(press 'N' to continue)";
                break;
            case 1:
                TutorialText.text = "We have provided you with your first shelf!\n\nPlace it somewhere in the shop by opening the inventory, and selecting the shelf from the structure tab.\n\n (Press N to continue)";
                break;
            case 2:
                TutorialText.text = "Good job!\n\nnow you have placed your first shelf, lets buy some stock to put on it.\n\nPress Q to access the shop screen, there you can purchase stock to sell for profit to customers.\n\n(Press N to continue)";
                break;
            case 3:
                TutorialText.text = "Nice!\n\nNow you can place those stock items on your shelf, by clicking on them in the inventory screen.\n\n stock items can only be placed on shelves.\n\n(Press N to continue)";
                break;
            case 4:
                TutorialText.text = "Finally, lets see if you can make profit from selling stock!\n\nThe goal is to make more than the initial money we have provided for you.\n\n(press N to end tutorial)";
                break;
        }
    }

    // Method to hide the tutorial
    public void HideTutorial()
    {
        TutorialText.text = "";
    }

    public void NextTutorialStep()
    {
        if (currentTutorialStep < 4) // Adjust based on number of steps
        {
            currentTutorialStep++;
            ShowTutorialStep(currentTutorialStep);
        }
        else
        {
            HideTutorial();
        }
    }

}
