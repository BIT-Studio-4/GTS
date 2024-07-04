using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [HideInInspector] public Button Button { get { return button; } set { button = value; } }
    [HideInInspector] public TextMeshProUGUI Text { get { return text; } set { text = value; } }
}
