using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button quitButton;
    [SerializeField] private GameObject selectOnOpen;

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
        quitButton.onClick.AddListener(OnQuit);
    }
    
    private void OnEnable()
    {
        Time.timeScale = 0;
        UIManager.Instance.EventSystemMain.SetSelectedGameObject(selectOnOpen);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void OnContinue()
    {
        UIManager.Instance.SetGUIState(UIType.Pause, false);
    }

    private void OnQuit()
    {
        SaveManager.Instance.SaveGame();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
