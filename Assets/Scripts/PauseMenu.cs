using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button tutorialButton;
    [SerializeField] Button quitButton;
    [SerializeField] private GameObject selectOnOpen;

    private List<AnimationTriggers> allButtonsAnimationTrigs = new();

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
        tutorialButton.onClick.AddListener(OnTutorial);
        quitButton.onClick.AddListener(OnQuit);

        allButtonsAnimationTrigs.Add(continueButton.GetComponentInChildren<Button>().animationTriggers);
        allButtonsAnimationTrigs.Add(quitButton.GetComponentInChildren<Button>().animationTriggers);
    }
    
    private void OnEnable()
    {
        InputDeviceManager.Instance.onGameDeviceChanged.AddListener(HandleInputDeviceType);
        Time.timeScale = 0;
        HandleInputDeviceType(); //set first selected if gamepad
    }

    private void OnDisable()
    {
        InputDeviceManager.Instance.onGameDeviceChanged.AddListener(HandleInputDeviceType);
        Time.timeScale = 1;
    }

    private void OnContinue()
    {
        UIManager.Instance.SetGUIState(UIType.Pause, false);
    }

    private void OnTutorial()
    {
        TutorialManager.Instance.Start();
    }

    private void OnQuit()
    {
        SaveManager.Instance.SaveGame();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    /// <summary>
    /// Switch all button hover effects to match input device
    /// </summary>
    private void HandleInputDeviceType()
    {
        if (InputDeviceManager.Instance.ActiveDevice == InputDevice.KeyboardMouse)
        {
            foreach (AnimationTriggers trigs in allButtonsAnimationTrigs)
            {
                trigs.highlightedTrigger = "Highlighted";
                trigs.selectedTrigger = "Normal";
            }
            UIManager.Instance.EventSystemMain.SetSelectedGameObject(null);
        }
        else if (InputDeviceManager.Instance.ActiveDevice == InputDevice.Gamepad)
        {
            foreach (AnimationTriggers trigs in allButtonsAnimationTrigs)
            {
                trigs.highlightedTrigger = "Normal";
                trigs.selectedTrigger = "Highlighted";
            }
            UIManager.Instance.EventSystemMain.SetSelectedGameObject(selectOnOpen);
        }
    }
}
