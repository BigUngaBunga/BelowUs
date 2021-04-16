// GENERATED AUTOMATICALLY FROM 'Assets/Input Actions/Menu Action.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MenuAction : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MenuAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Menu Action"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""e4cb3438-d6f0-48e6-82b3-ba408c71b137"",
            ""actions"": [
                {
                    ""name"": ""MenuButton"",
                    ""type"": ""Button"",
                    ""id"": ""b8c73735-f64b-49b7-8d95-5c9e098b1927"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""531aee31-09c4-438f-8dee-27d4b64475eb"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MenuButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_MenuButton = m_Menu.FindAction("MenuButton", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_MenuButton;
    public struct MenuActions
    {
        private @MenuAction m_Wrapper;
        public MenuActions(@MenuAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @MenuButton => m_Wrapper.m_Menu_MenuButton;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @MenuButton.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenuButton;
                @MenuButton.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenuButton;
                @MenuButton.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenuButton;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MenuButton.started += instance.OnMenuButton;
                @MenuButton.performed += instance.OnMenuButton;
                @MenuButton.canceled += instance.OnMenuButton;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IMenuActions
    {
        void OnMenuButton(InputAction.CallbackContext context);
    }
}
