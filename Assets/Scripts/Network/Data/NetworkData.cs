using Unity.Netcode;
using UnityEngine.Events;

namespace Network.Data
{
    public abstract class NetworkData<T> : NetworkBehaviour where T : struct
    {
        public UnityEvent<T> OnValueChanged = new ();
        public UnityEvent OnNetworkSpawned = new ();
        
        private T value;
        public T Value
        {
            get => value;
            protected set
            {
                if (this.value.Equals(value)) return;
                
                this.value = value;
                OnValueChanged.Invoke(value);
            }
        }
        
        public override void OnNetworkSpawn()
        {
            Value = GetValue();
            OnNetworkSpawned.Invoke();
            
            base.OnNetworkSpawn();
        }

        public abstract T GetValue();
    }
}