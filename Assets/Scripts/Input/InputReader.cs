using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public Action<Vector2> MoveEvent;
    public Action<bool> PrimaryFireEvent;
    public Vector2 AimPosition { get; private set; }
    private Controls _controls;

    void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Enable();
    }
    void OnDisable()
    {
        _controls?.Player.Disable();
        _controls?.Disable();
    }

    void IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void IPlayerActions.OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            PrimaryFireEvent?.Invoke(true);
        else if (context.canceled)
            PrimaryFireEvent?.Invoke(false);
    }

    void IPlayerActions.OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}
