//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scrips/PlayerInputAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputAction : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputAction"",
    ""maps"": [
        {
            ""name"": ""PlayerControls"",
            ""id"": ""5aaa83e5-ae18-4e4d-898e-fb8718a710c1"",
            ""actions"": [
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""Value"",
                    ""id"": ""9edabb7c-6c69-4d62-b410-26fb06cca714"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""KeyboardMove"",
                    ""type"": ""Value"",
                    ""id"": ""5838900b-f5ee-4751-a728-d9a9f5f02158"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""FireMain"",
                    ""type"": ""PassThrough"",
                    ""id"": ""39e0bf72-6170-47db-acc5-67dbc7316c1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""FireSecondary"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e4a4e3b5-1279-4864-bfe7-2b60ba9faede"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangeSpeed"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a3b74c4a-9c46-426e-af2f-f0e22021eda7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Freelook"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ccff64dd-4d0f-47ca-a7b3-5cf2fd082966"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Yaw"",
                    ""type"": ""Button"",
                    ""id"": ""87c9d584-8a47-4c73-a472-5823ec54ddc0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""IngameMenu"",
                    ""type"": ""Button"",
                    ""id"": ""fc443e02-5276-4d61-b4fd-a5a8252da6e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""3da5a8eb-0d00-4f0f-bed1-64efc7009574"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""05570d41-1524-47f9-b9d6-c15ce7a7c47d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""15693d8d-2b8a-4220-be7c-9fbae596ae52"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5e8be585-330c-411e-ac8f-dac1d2e2c847"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4f961161-cf83-412a-8ccc-52e9a19ee7b8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""33b4b013-0b30-4843-97ff-672e3c4842e6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a5c3327f-f784-46e0-b7d3-3c023aff3f1c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a42f8faa-5b98-4cc5-a267-eb595c548a13"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FireMain"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2c5a3a0-9fc8-4ff8-93a6-57b788bf3ab9"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FireSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""faadadbb-95f0-4093-a35e-f9291850f4d8"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Freelook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""294fca9c-c63e-47bf-ad37-dda32fb0cd48"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cde7ba98-6312-4b11-a9e2-4c227b4f43c3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9cf4ed4f-4c79-41f7-a386-729d5a6e25ee"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IngameMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e313036e-4fc8-4e90-ba9c-e416cdd7b7af"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""21fb0f2b-356d-4769-9848-6bbe05c3448e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpeed"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""224326af-bf2b-4917-ae85-2a019c496866"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpeed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""81e5773b-e754-4dbf-95ba-6b6f398575db"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpeed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerControls
        m_PlayerControls = asset.FindActionMap("PlayerControls", throwIfNotFound: true);
        m_PlayerControls_MouseMove = m_PlayerControls.FindAction("MouseMove", throwIfNotFound: true);
        m_PlayerControls_KeyboardMove = m_PlayerControls.FindAction("KeyboardMove", throwIfNotFound: true);
        m_PlayerControls_FireMain = m_PlayerControls.FindAction("FireMain", throwIfNotFound: true);
        m_PlayerControls_FireSecondary = m_PlayerControls.FindAction("FireSecondary", throwIfNotFound: true);
        m_PlayerControls_ChangeSpeed = m_PlayerControls.FindAction("ChangeSpeed", throwIfNotFound: true);
        m_PlayerControls_Freelook = m_PlayerControls.FindAction("Freelook", throwIfNotFound: true);
        m_PlayerControls_Yaw = m_PlayerControls.FindAction("Yaw", throwIfNotFound: true);
        m_PlayerControls_IngameMenu = m_PlayerControls.FindAction("IngameMenu", throwIfNotFound: true);
        m_PlayerControls_SwitchWeapon = m_PlayerControls.FindAction("SwitchWeapon", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerControls
    private readonly InputActionMap m_PlayerControls;
    private IPlayerControlsActions m_PlayerControlsActionsCallbackInterface;
    private readonly InputAction m_PlayerControls_MouseMove;
    private readonly InputAction m_PlayerControls_KeyboardMove;
    private readonly InputAction m_PlayerControls_FireMain;
    private readonly InputAction m_PlayerControls_FireSecondary;
    private readonly InputAction m_PlayerControls_ChangeSpeed;
    private readonly InputAction m_PlayerControls_Freelook;
    private readonly InputAction m_PlayerControls_Yaw;
    private readonly InputAction m_PlayerControls_IngameMenu;
    private readonly InputAction m_PlayerControls_SwitchWeapon;
    public struct PlayerControlsActions
    {
        private @PlayerInputAction m_Wrapper;
        public PlayerControlsActions(@PlayerInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseMove => m_Wrapper.m_PlayerControls_MouseMove;
        public InputAction @KeyboardMove => m_Wrapper.m_PlayerControls_KeyboardMove;
        public InputAction @FireMain => m_Wrapper.m_PlayerControls_FireMain;
        public InputAction @FireSecondary => m_Wrapper.m_PlayerControls_FireSecondary;
        public InputAction @ChangeSpeed => m_Wrapper.m_PlayerControls_ChangeSpeed;
        public InputAction @Freelook => m_Wrapper.m_PlayerControls_Freelook;
        public InputAction @Yaw => m_Wrapper.m_PlayerControls_Yaw;
        public InputAction @IngameMenu => m_Wrapper.m_PlayerControls_IngameMenu;
        public InputAction @SwitchWeapon => m_Wrapper.m_PlayerControls_SwitchWeapon;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControlsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControlsActions instance)
        {
            if (m_Wrapper.m_PlayerControlsActionsCallbackInterface != null)
            {
                @MouseMove.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMouseMove;
                @MouseMove.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMouseMove;
                @MouseMove.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMouseMove;
                @KeyboardMove.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnKeyboardMove;
                @KeyboardMove.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnKeyboardMove;
                @KeyboardMove.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnKeyboardMove;
                @FireMain.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireMain;
                @FireMain.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireMain;
                @FireMain.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireMain;
                @FireSecondary.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireSecondary;
                @FireSecondary.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireSecondary;
                @FireSecondary.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFireSecondary;
                @ChangeSpeed.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnChangeSpeed;
                @ChangeSpeed.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnChangeSpeed;
                @ChangeSpeed.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnChangeSpeed;
                @Freelook.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFreelook;
                @Freelook.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFreelook;
                @Freelook.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFreelook;
                @Yaw.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnYaw;
                @Yaw.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnYaw;
                @Yaw.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnYaw;
                @IngameMenu.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnIngameMenu;
                @IngameMenu.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnIngameMenu;
                @IngameMenu.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnIngameMenu;
                @SwitchWeapon.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSwitchWeapon;
                @SwitchWeapon.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSwitchWeapon;
                @SwitchWeapon.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSwitchWeapon;
            }
            m_Wrapper.m_PlayerControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
                @KeyboardMove.started += instance.OnKeyboardMove;
                @KeyboardMove.performed += instance.OnKeyboardMove;
                @KeyboardMove.canceled += instance.OnKeyboardMove;
                @FireMain.started += instance.OnFireMain;
                @FireMain.performed += instance.OnFireMain;
                @FireMain.canceled += instance.OnFireMain;
                @FireSecondary.started += instance.OnFireSecondary;
                @FireSecondary.performed += instance.OnFireSecondary;
                @FireSecondary.canceled += instance.OnFireSecondary;
                @ChangeSpeed.started += instance.OnChangeSpeed;
                @ChangeSpeed.performed += instance.OnChangeSpeed;
                @ChangeSpeed.canceled += instance.OnChangeSpeed;
                @Freelook.started += instance.OnFreelook;
                @Freelook.performed += instance.OnFreelook;
                @Freelook.canceled += instance.OnFreelook;
                @Yaw.started += instance.OnYaw;
                @Yaw.performed += instance.OnYaw;
                @Yaw.canceled += instance.OnYaw;
                @IngameMenu.started += instance.OnIngameMenu;
                @IngameMenu.performed += instance.OnIngameMenu;
                @IngameMenu.canceled += instance.OnIngameMenu;
                @SwitchWeapon.started += instance.OnSwitchWeapon;
                @SwitchWeapon.performed += instance.OnSwitchWeapon;
                @SwitchWeapon.canceled += instance.OnSwitchWeapon;
            }
        }
    }
    public PlayerControlsActions @PlayerControls => new PlayerControlsActions(this);
    public interface IPlayerControlsActions
    {
        void OnMouseMove(InputAction.CallbackContext context);
        void OnKeyboardMove(InputAction.CallbackContext context);
        void OnFireMain(InputAction.CallbackContext context);
        void OnFireSecondary(InputAction.CallbackContext context);
        void OnChangeSpeed(InputAction.CallbackContext context);
        void OnFreelook(InputAction.CallbackContext context);
        void OnYaw(InputAction.CallbackContext context);
        void OnIngameMenu(InputAction.CallbackContext context);
        void OnSwitchWeapon(InputAction.CallbackContext context);
    }
}
