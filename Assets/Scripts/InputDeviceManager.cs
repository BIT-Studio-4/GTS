using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InputDevice
{
    KeyboardMouse,
    Gamepad
}

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance;

    [HideInInspector] public UnityEvent onGameDeviceChanged;

    private InputDevice activeDevice;

    public InputDevice ActiveDevice
    {
        get => activeDevice;
        set
        {
            // only change when the previous input device is different
            if (value != activeDevice)
                ChangeActiveDevice(value);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // add listener that will run whenever user interacts with any action
        InputSystem.onActionChange += DetectInputDevice;
    }

    // code snippet from code monkey tutorial (but i made it better)
    // https://youtu.be/j2XyzSAD4VU
    private void DetectInputDevice(object arg1, InputActionChange inputActionChange)
    {
        // only run method when user performs an action (as opposed to cancel or enable etc)
        if (inputActionChange != InputActionChange.ActionPerformed || arg1 is not InputAction)
            return;
        // get action that was performed
        InputAction inputAction = arg1 as InputAction;
        // ignore virtual mouse
        if (inputAction.activeControl.device.displayName == "VirtualMouse")
            return;
        
        if (inputAction.activeControl.device is Gamepad)
            ActiveDevice = InputDevice.Gamepad;
        else 
            ActiveDevice = InputDevice.KeyboardMouse;
    }

    private void ChangeActiveDevice(InputDevice inputDevice)
    {
        activeDevice = inputDevice;
        onGameDeviceChanged?.Invoke();
    }
}
