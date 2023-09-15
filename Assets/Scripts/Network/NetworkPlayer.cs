using Components.Controller;
using Components.UI.Game.Inventory;
using Interfaces;
using Managers;
using SimpleJSON;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Network
{
    public class NetworkPlayer : NetworkBehaviour, ISavable
    {
        #region EDITOR EXPOSED FIELDS

        public NetworkVariable<string> uuid = new NetworkVariable<string>("", 
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        [Header("References")]
        [SerializeField] private GameObject _localOnlyObjects;
        [SerializeField] private GameObject _remoteOnlyObjects;
        [SerializeField] private BaseInventory _inventory;
        [SerializeField] private CanvasGroup _inventoryCg;
        
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
            // mouseLook.Call_Look(input);
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
        
        public void Call_Inventory(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            if (ctx.performed) // Every frame while the button is held down
            {
                bool isInventoryOpen = _inventoryCg.IsVisible();
                _inventoryCg.SetVisibility(!isInventoryOpen);
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

            MSave.Instance.players.Add(this);
            if (IsOwner)
            {
                uuid.Value = ClientPref.GetUUID();
            }
        }
        
        #endregion
        
        #region SAVE SYSTEM

        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            
            json.Add("id", uuid.Value);
            json.Add("inventory", _inventory.Save());
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("id", "");
            json.Add("inventory", _inventory.GetDefaultSave());
            
            return json;
        }

        public void Load(JSONObject json)
        {
            _inventory.Load(json["inventory"].AsObject);
        }

        #endregion
    }
}