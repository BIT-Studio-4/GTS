using UnityEngine;

public class UIStyling : MonoBehaviour
{
    public static UIStyling Instance;

    // Add new colours in here for UI
    [SerializeField] private Color buttonValidColor;
    [SerializeField] private Color buttonAddColor;
    [SerializeField] private Color buttonSubtractColor;
    [SerializeField] private Color buttonDeselectedColor;
    [SerializeField] private Color buttonInvalidColor;

    public Color ButtonValidColor {  get => buttonValidColor; }
    public Color ButtonAddColor {  get => buttonAddColor; }
    public Color ButtonNegativeColor {  get => buttonSubtractColor; }
    public Color ButtonDeselectedColor {  get => buttonDeselectedColor; }
    public Color ButtonInvalidColor {  get => buttonInvalidColor; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Multiple instances UIStyling found, deleting instance attached to {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
