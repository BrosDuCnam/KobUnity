using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Components.Proto
{
    public class MovingPlatform : NetworkBehaviour
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private float speed = 1f;
        
        private int _currentPoint;

        public UnityEvent<Vector3> OnMove { get; } = new UnityEvent<Vector3>();
        private Vector3 _lastPosition;

        private void Update()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            
            Vector3 delta = transform.position - _lastPosition;
            OnMove?.Invoke(delta);
            _lastPosition = transform.position;

            if (!IsServer) return;
            
            // transform.position = Vector3.MoveTowards(transform.position, points[_currentPoint].position, speed * Time.deltaTime);
            rb.MovePosition(Vector3.MoveTowards(transform.position, points[_currentPoint].position, speed * Time.deltaTime));
            if (transform.position == points[_currentPoint].position)
            {
                _currentPoint++;
                if (_currentPoint >= points.Length)
                {
                    _currentPoint = 0;
                }
            }
        }
    }
}