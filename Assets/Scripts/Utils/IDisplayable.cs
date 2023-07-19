using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public interface IDisplayable
    {
        protected CanvasGroup CanvasGroup { get; }
        
        public Tween Display(bool display, bool changeActive, bool instant = false)
        {
            if (instant)
            {
                CanvasGroup.alpha = display ? 1f : 0f;
                CanvasGroup.gameObject.SetActive(display);
                return null;
            }
            
            return CanvasGroup.DOFade(display ? 1f : 0f, 0.3f).OnKill(() =>
            {
                if (changeActive)
                    CanvasGroup.gameObject.SetActive(display);
            });
        }
    }
}