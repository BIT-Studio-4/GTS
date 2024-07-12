using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Button subtractButton;
    [SerializeField] private Button addButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI countText;

    [HideInInspector] public Button SubtractButton { get => subtractButton; set => subtractButton = value; }
    [HideInInspector] public Button AddButton { get => addButton; set => addButton = value; }
    [HideInInspector] public TextMeshProUGUI NameText { get => nameText; set => nameText = value; }
    [HideInInspector] public TextMeshProUGUI PriceText { get => priceText; set => priceText = value; }
    [HideInInspector] public TextMeshProUGUI CountText { get => countText; set => countText = value; }
}
