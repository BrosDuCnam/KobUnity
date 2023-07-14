using UnityEngine;

namespace Components
{
    /// <summary>
    /// Class to handle all collider events.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderEvents : MonoBehaviour
    {
        [HideInInspector] public Collider Collider;
        
        // Events
        public delegate void TriggerEnter(Collider other);
        public event TriggerEnter OnTriggerEnterEvent;
        public event TriggerEnter OnTriggerStayEvent;
        public event TriggerEnter OnTriggerExitEvent;
        
        public delegate void CollisionEnter(Collision other);
        public event CollisionEnter OnCollisionEnterEvent;
        public event CollisionEnter OnCollisionStayEvent;
        public event CollisionEnter OnCollisionExitEvent;
        
        private void Awake()
        {
            Collider = GetComponent<Collider>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }
        
        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            OnCollisionEnterEvent?.Invoke(other);
        }
        
        private void OnCollisionStay(Collision other)
        {
            OnCollisionStayEvent?.Invoke(other);
        }
        
        private void OnCollisionExit(Collision other)
        {
            OnCollisionExitEvent?.Invoke(other);
        }
    }
}