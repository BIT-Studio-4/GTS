using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(VirtualMouseInput))]
public class VirtualMouseUI : MonoBehaviour
{
    private VirtualMouseInput virtualMouseInput;
    
    private void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
    }

    private void OnEnable()
    {
        StartCoroutine(EnableCursor());
    }

    IEnumerator EnableCursor()
    {
        yield return new WaitUntil(() => virtualMouseInput != null);

        Vector2 virtualMousePosition = new (Screen.width / 2, Screen.height / 2);
        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }

    // code snippet from this Code Monkey tutorial
    // https://youtu.be/j2XyzSAD4VU
    private void LateUpdate()
    {
        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0, Screen.width);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0, Screen.height);
        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }
}
