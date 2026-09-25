// Script from git-amend

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace PlayerMovement
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "PlayerMovement/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };

        InputSystem_Actions inputActions;

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>(); 

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new InputSystem_Actions();
                inputActions.Player.SetCallbacks(instance: this); 
            }
            inputActions.Enable();
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable(); 
        }

        void OnDisable()
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context)); 
        }

        // Helper functionn that 
        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled: 
                    DisableMouseControlCamera.Invoke(); 
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>()); 
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            //noop
        }
    }
}
