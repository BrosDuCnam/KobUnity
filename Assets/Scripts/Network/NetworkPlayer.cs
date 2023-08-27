using Components.Controller;
using Unity.Netcode;
using UnityEngine;

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
        
        public BaseCharacterController movement { get; private set; }
        public MouseLook mouseLook { get; private set; }
        public CharacterMovement characterMovement { get; private set; }

        #endregion
        
        #region MONOBEHAVIOUR
        
        private void Awake()
        {
            movement = GetComponent<BaseCharacterController>();
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
                movement.enabled = true;
                characterMovement.enabled = true;
                
                _localOnlyObjects.SetActive(true);
                _remoteOnlyObjects.SetActive(false);
            }
            else
            {
                mouseLook.enabled = false;
                movement.enabled = false;
                characterMovement.enabled = false;
                
                _localOnlyObjects.SetActive(false);
                _remoteOnlyObjects.SetActive(true);
            }
        }
        
        #endregion
    }
}