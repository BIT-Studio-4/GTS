using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    }

    private void Start()
    {
        //event listener to change display
        GameManager.Instance.OnMoneyChange.AddListener(MoneyChange);
    }

    //method to update money when changed
    private void MoneyChange()
    {
        moneyDisplay.text = $"${GameManager.Instance.Money}";
    }
}
