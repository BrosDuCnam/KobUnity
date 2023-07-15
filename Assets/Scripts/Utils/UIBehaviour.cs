using UnityEngine;

namespace Utils
{
    public abstract class UIBehaviour<T> : MonoBehaviour where T : struct
    {
        public abstract void Refresh(T data);
    }
}