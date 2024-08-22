using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Stores information about things on the inventory grid slots. This is so it can be easily accessed when setting it in InventoryManager
public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI salePriceText;
    [HideInInspector] public Button Button { get => button;  set => button = value; } 
    [HideInInspector] public TextMeshProUGUI Text { get => text; set => text = value; }
    [HideInInspector] public TextMeshProUGUI CountText { get => countText; set => countText = value; }
    [HideInInspector] public TextMeshProUGUI SalePriceText { get => salePriceText; set => salePriceText = value; }
}
