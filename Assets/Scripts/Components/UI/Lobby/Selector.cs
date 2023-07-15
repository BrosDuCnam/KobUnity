using DG.Tweening;
using UnityEngine;

namespace Components.UI.Lobby
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] public RectTransform rectTransform;
        
        public bool IsActived => rectTransform.localScale.y > 0f;
        
        public void SetActive(bool active)
        {
            if (IsActived == active) return;
            
            float targetScale = active ? 1f : 0f;
            rectTransform.DOScaleY(targetScale, 0.1f);
        }
    }
}