using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Components
{
    public class EntityPlayer : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private List<GameObject> clientSideObjects;
        [SerializeField] private List<GameObject> otherSideObjects;
        [SerializeField] private CharacterController characterController;
        
        private void Awake()
        {
            characterController.enabled = false;
        }
        
        /// <summary>
        /// Called when the object is spawned on the network.
        /// Sets the active state of the objects based on whether the player is the local player or not.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsLocalPlayer)
            {
                SetObjectsActive(clientSideObjects, true);
                SetObjectsActive(otherSideObjects, false);
                
                characterController.enabled = true;
                
                // Set the player's position to the spawn position.
                transform.position = new Vector3(0, 2, 0);
            }
            else
            {
                SetObjectsActive(otherSideObjects, true);
                SetObjectsActive(clientSideObjects, false);
            }
        }

        /// <summary>
        /// Sets the active state of the specified objects to the specified value.
        /// </summary>
        /// <param name="objects">The objects to set the active state of.</param>
        /// <param name="active">The active state to set the objects to.</param>
        private void SetObjectsActive(IEnumerable<GameObject> objects, bool active)
        {
            foreach (var obj in objects)
            {
                obj.SetActive(active);
            }
        }
    }
}