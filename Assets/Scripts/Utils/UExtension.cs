using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public static class UExtension
    {
        public static Tween SetVisibility(this CanvasGroup canvasGroup, bool value, float duration = 0.15f)
        {
            return canvasGroup.DOFade(value ? 1 : 0, duration);
        }
        
        public static bool IsVisible(this CanvasGroup canvasGroup)
        {
            return canvasGroup.alpha > 0;
        }
    }
}