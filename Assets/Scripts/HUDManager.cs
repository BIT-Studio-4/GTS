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
    private Boolean errorPopupUp;
    private float popupTime;
    [SerializeField] private float popupDelay;
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

    }

    private void Start()
    {
        //event listener to change display
        GameManager.Instance.OnMoneyChange.AddListener(MoneyChange);
        errorPopupUp = false;
        versionText.text = $"Version Number: {Application.version}";
    }

    private void Update()
    {
        if (errorPopupUp == true && Time.time > popupTime + popupDelay)
        {
            errorPopup.style.display = DisplayStyle.None;
            errorPopupUp = false;
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
        popupTime = Time.time;
    }
}
