using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    [SerializeField]
    private GameObject hudObject;
    private VisualElement hud;
    private VisualElement moneyContainer;
    private Label moneyDisplay;
    private VisualElement errorPopup;
    private Label errorMessage;
    private VisualElement versionContainer;
    private Label versionText;
    private VisualElement saveGamePopup;
    private Boolean errorPopupUp;
    private Boolean saveGamePopupUp;
    private float errorPopupTime;
    private float saveGamePopupTime;
    [SerializeField] private float errorPopupDelay;
    [SerializeField] private float saveGamePopupDelay;
    [SerializeField] private PlaceObject placement;

    void Awake()
    {
        Instance = this;
        hud = hudObject.GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        //hud elements selecting
        moneyContainer = hud.Q<VisualElement>("moneyContainer");
        moneyDisplay = moneyContainer.Q<Label>("moneyDisplay");
        errorPopup = hud.Q<VisualElement>("errorPopup");
        errorMessage = errorPopup.Q<Label>("errorText");
        versionContainer = hud.Q<VisualElement>("versionNumber");
        versionText = versionContainer.Q<Label>("versionText");
        saveGamePopup = hud.Q<VisualElement>("savedGame");
    }

    private void Start()
    {
        //event listener to change display
        GameManager.Instance.OnMoneyChange.AddListener(MoneyChange);
        errorPopupUp = false;
        versionText.text = $"Version Number: {Application.version}";

        saveGamePopupUp = false;
    }

    private void Update()
    {
        if (errorPopupUp == true && Time.time > errorPopupTime + errorPopupDelay)
        {
            errorPopup.style.display = DisplayStyle.None;
            errorPopupUp = false;
        }
        if (saveGamePopupUp && Time.time > saveGamePopupTime + saveGamePopupDelay)
        {
            saveGamePopup.style.display = DisplayStyle.None;
            saveGamePopupUp = false;
        }
    }

    //method to update money when changed
    private void MoneyChange()
    {
        moneyDisplay.text = $"${GameManager.Instance.Money}";
    }

    //method to popup a message when an error occurs
    public void ErrorPopup(string message){
        errorMessage.text = message;
        errorPopup.style.display = DisplayStyle.Flex;
        errorPopupUp = true;
        errorPopupTime = Time.time;
    }

    public void SaveGame()
    {
        saveGamePopup.style.display = DisplayStyle.Flex;
        saveGamePopupUp = true;
        saveGamePopupTime = Time.time;
    }
}
