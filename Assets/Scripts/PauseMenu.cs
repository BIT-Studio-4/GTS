using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button quitButton;
    
    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
        quitButton.onClick.AddListener(OnQuit);
    }
    
    private void OnContinue()
    {
        Debug.Log("Continue game!");
        
    }
    
    private void OnQuit()
    {
        Debug.Log("Quit game!");
        
    }
}
