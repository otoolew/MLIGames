// GENERATED AUTOMATICALLY FROM 'Assets/Code/Input/CharacterInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CharacterInputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""abb029b6-1827-4d8c-8ec2-fadf44d0971e"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""0409429c-89f4-4317-84ef-530be29dab60"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""85bb94cc-5ec3-439d-9b87-077fa007477f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseInteraction"",
                    ""type"": ""Button"",
                    ""id"": ""224015df-7d35-4576-b6ce-265e56baf55a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left_PullTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""4c2bf812-3025-4370-adae-abacb1233cf3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Right_PullTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""45976f15-0c56-4d1e-8824-b7510a14b9e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Left_Reload"",
                    ""type"": ""Button"",
                    ""id"": ""665dd4f3-cc9d-4e1b-b9ec-87ee20ce87c2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right_Reload"",
                    ""type"": ""Button"",
                    ""id"": ""5c7c4d0f-4299-4c29-a702-a7b392df7be3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""cd1ee628-6d80-4307-ba61-ef534a424c2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a43a1391-8c06-40c4-9747-c72fdf8d0710"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left_PullTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4854b554-a426-4696-8fd2-2c57e5325009"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left_PullTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3643de7-8185-4c74-a36d-e43ae3c27f80"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right_PullTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34ad7a80-8d7b-4459-aa26-f99c28dbd4c8"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right_PullTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fad44bc6-293d-4aad-a38d-b3a37bb94c8a"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left_Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94673672-f70b-40e9-8b00-4b61b121dce9"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left_Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1328fe9-e344-4b10-93f4-642e2274c6c2"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right_Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aef69867-710d-4bb1-8a03-1dbb5ffabc39"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right_Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f718adf6-c7ac-4981-94c5-17c47f034905"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2d0f96a-c605-40f0-bc11-0b2d4b243a87"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1c24173-aeb9-4bc3-8bab-0822fc6d1691"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Dpad"",
                    ""id"": ""d848cff0-edf8-42af-8131-06085846087f"",
                    ""path"": ""Dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c17cb7e2-ac16-4808-b0f7-67d8245d089d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c9990732-6635-4943-8860-58e054a548b0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a2d66f18-faec-4be7-805e-db5c0ccefff6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2c943ec1-ffe2-4dd1-a38d-5500116d0e8b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a512d072-9551-406e-b733-31e027d83e53"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f33e9087-75ea-4ee5-9097-9413f1dcc241"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseInteraction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62873c15-9469-4a13-9fc2-3fe2b7a43751"",
                    ""path"": ""<XInputController>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseInteraction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_UseInteraction = m_Player.FindAction("UseInteraction", throwIfNotFound: true);
        m_Player_Left_PullTrigger = m_Player.FindAction("Left_PullTrigger", throwIfNotFound: true);
        m_Player_Right_PullTrigger = m_Player.FindAction("Right_PullTrigger", throwIfNotFound: true);
        m_Player_Left_Reload = m_Player.FindAction("Left_Reload", throwIfNotFound: true);
        m_Player_Right_Reload = m_Player.FindAction("Right_Reload", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_UseInteraction;
    private readonly InputAction m_Player_Left_PullTrigger;
    private readonly InputAction m_Player_Right_PullTrigger;
    private readonly InputAction m_Player_Left_Reload;
    private readonly InputAction m_Player_Right_Reload;
    private readonly InputAction m_Player_Dash;
    public struct PlayerActions
    {
        private @CharacterInputActions m_Wrapper;
        public PlayerActions(@CharacterInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @UseInteraction => m_Wrapper.m_Player_UseInteraction;
        public InputAction @Left_PullTrigger => m_Wrapper.m_Player_Left_PullTrigger;
        public InputAction @Right_PullTrigger => m_Wrapper.m_Player_Right_PullTrigger;
        public InputAction @Left_Reload => m_Wrapper.m_Player_Left_Reload;
        public InputAction @Right_Reload => m_Wrapper.m_Player_Right_Reload;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @UseInteraction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseInteraction;
                @UseInteraction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseInteraction;
                @UseInteraction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseInteraction;
                @Left_PullTrigger.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_PullTrigger;
                @Left_PullTrigger.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_PullTrigger;
                @Left_PullTrigger.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_PullTrigger;
                @Right_PullTrigger.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_PullTrigger;
                @Right_PullTrigger.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_PullTrigger;
                @Right_PullTrigger.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_PullTrigger;
                @Left_Reload.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_Reload;
                @Left_Reload.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_Reload;
                @Left_Reload.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeft_Reload;
                @Right_Reload.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_Reload;
                @Right_Reload.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_Reload;
                @Right_Reload.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRight_Reload;
                @Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @UseInteraction.started += instance.OnUseInteraction;
                @UseInteraction.performed += instance.OnUseInteraction;
                @UseInteraction.canceled += instance.OnUseInteraction;
                @Left_PullTrigger.started += instance.OnLeft_PullTrigger;
                @Left_PullTrigger.performed += instance.OnLeft_PullTrigger;
                @Left_PullTrigger.canceled += instance.OnLeft_PullTrigger;
                @Right_PullTrigger.started += instance.OnRight_PullTrigger;
                @Right_PullTrigger.performed += instance.OnRight_PullTrigger;
                @Right_PullTrigger.canceled += instance.OnRight_PullTrigger;
                @Left_Reload.started += instance.OnLeft_Reload;
                @Left_Reload.performed += instance.OnLeft_Reload;
                @Left_Reload.canceled += instance.OnLeft_Reload;
                @Right_Reload.started += instance.OnRight_Reload;
                @Right_Reload.performed += instance.OnRight_Reload;
                @Right_Reload.canceled += instance.OnRight_Reload;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnUseInteraction(InputAction.CallbackContext context);
        void OnLeft_PullTrigger(InputAction.CallbackContext context);
        void OnRight_PullTrigger(InputAction.CallbackContext context);
        void OnLeft_Reload(InputAction.CallbackContext context);
        void OnRight_Reload(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
    }
}
