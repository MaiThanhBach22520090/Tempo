using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    private PlayerInputActions actions;

    private const string PLAYER_PREFS_BINDING = "InputBindings";

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Left_Tap,
        Right_Tap
    }


    private void Awake()
    {
        actions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))
        {
            actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDING));
        }

        actions.Player.Enable();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public Vector2 GetMovementVector()
    {
        return actions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetMousePosition()
    {
        return actions.Player.Look.ReadValue<Vector2>();
    }

    public bool TapLeft()
    {
        return actions.Player.Left_Tap.triggered;
    }

    public bool TapRight()
    {
        return actions.Player.Right_Tap.triggered;
    }

    public string GetBidingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Move_Up:
                return actions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return actions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return actions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return actions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Left_Tap:
                return actions.Player.Left_Tap.bindings[0].ToDisplayString();
            case Binding.Right_Tap:
                return actions.Player.Right_Tap.bindings[0].ToDisplayString();
            default:
                return "None";
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        InputAction action;
        int bindingIndex;

        switch (binding)
        {
            case Binding.Move_Up:
                action = actions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                action = actions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                action = actions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                action = actions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Left_Tap:
                action = actions.Player.Left_Tap;
                bindingIndex = 0;
                break;
            case Binding.Right_Tap:
                action = actions.Player.Right_Tap;
                bindingIndex = 0;
                break;
            default:
                return;
        }

        // Disable the action before rebinding
        actions.Player.Disable();

        // Start the rebinding process
        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithCancelingThrough("<Keyboard>/escape") // Optional: allows canceling with Escape
            .OnComplete(callback =>
            {
                actions.Player.Enable();
                callback.Dispose();
                onActionRebound?.Invoke();

                actions.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDING, actions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}
