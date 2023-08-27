using Unity.Netcode;
using UnityEngine;

namespace Components
{
    public class TestPlayer : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 5f;
        
        private void Update()
        {
            if (!IsLocalPlayer) return;
            
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 direction = input.normalized;
            Vector3 velocity = direction * speed;
            Vector3 moveAmount = velocity * Time.deltaTime;
            
            transform.position += moveAmount;
        }
    }
}