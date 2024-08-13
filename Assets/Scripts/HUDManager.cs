using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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
        errorMessage = errorMessage.Q<Label>("errorText");
    }

    private void Start()
    {
        //event listener to change display
        GameManager.Instance.OnMoneyChange.AddListener(MoneyChange);
        placement.incorrectPlacement.AddListener(ErrorPopup);

    }

    //method to update money when changed
    private void MoneyChange()
    {
        moneyDisplay.text = $"${GameManager.Instance.Money}";
    }

    //method to popup a message when an error occurs
    private void ErrorPopup(string message){
        errorMessage.text = message;
        errorPopup.style.display = DisplayStyle.Flex;
        Debug.Log(message);
    }
}
