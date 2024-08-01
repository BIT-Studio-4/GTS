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

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        hud = hudObject.GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        moneyContainer = hud.Q<VisualElement>("moneyContainer");
        moneyDisplay = moneyContainer.Q<Label>("moneyDisplay");
    }

    private void Start()
    {
        GameManager.Instance.OnMoneyChange.AddListener(MoneyChange);
    }

    private void MoneyChange()
    {
        moneyDisplay.text = $"${GameManager.Instance.Money}";
    }
}
