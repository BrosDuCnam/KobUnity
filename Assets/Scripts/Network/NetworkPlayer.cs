using Components.Controller;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [SerializeField]
        private GameObject _localOnlyObjects;
        
        [SerializeField]
        private GameObject _remoteOnlyObjects;

        #endregion
        
        #region PROPERTIES
        
        public FirstPersonController controller { get; private set; }
        public MouseLook mouseLook { get; private set; }
        public CharacterMovement characterMovement { get; private set; }

        #endregion
        
        #region INPUT SYSTEM

        public void Call_MoveHorizontal(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            Vector2 input = ctx.ReadValue<Vector2>();
            controller.Call_MoveHorizontal(input);
        }
        
        public void Call_Look(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            Vector2 input = ctx.ReadValue<Vector2>();
            mouseLook.Call_Look(input);
        }
        
        public void Call_Jump(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            if (ctx.performed) // Every frame while the button is held down
            {
                controller.Call_Jump(true);
            }
            else if (ctx.canceled) // When the button is released
            {
                controller.Call_Jump(false);
            }
        }
        
        public void Call_Crouch(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            if (ctx.performed) // Every frame while the button is held down
            {
                controller.Call_Crouch(true);
            }
            else if (ctx.canceled) // When the button is released
            {
                controller.Call_Crouch(false);
            }
        }
        
        public void Call_Run(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            if (ctx.performed) // Every frame while the button is held down
            {
                controller.Call_Run(true);
            }
            else if (ctx.canceled) // When the button is released
            {
                controller.Call_Run(false);
            }
        }
        
        #endregion
        
        #region MONOBEHAVIOUR
        
        private void Awake()
        {
            controller = GetComponent<FirstPersonController>();
            mouseLook = GetComponentInChildren<MouseLook>();
            characterMovement = GetComponent<CharacterMovement>();
        }
        
        #endregion
        
        #region NETWORK BEHAVIOUR
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                mouseLook.enabled = true;
                controller.enabled = true;
                characterMovement.enabled = true;
                
                _localOnlyObjects.SetActive(true);
                _remoteOnlyObjects.SetActive(false);
            }
            else
            {
                mouseLook.enabled = false;
                controller.enabled = false;
                characterMovement.enabled = false;
                
                _localOnlyObjects.SetActive(false);
                _remoteOnlyObjects.SetActive(true);
            }
        }
        
        #endregion
    }
}