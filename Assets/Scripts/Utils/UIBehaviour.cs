using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
    public interface UIBehaviour<in T> where T : struct
    {
        public abstract void Refresh(T data);
        
        public MonoBehaviour GetMonoBehaviour()
        {
            return (MonoBehaviour) this;
        }
    }
}