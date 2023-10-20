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
        
        public static int GetStableHashCode(this string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i+1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i+1];
                }

                return hash1 + (hash2*1566083941);
            }
        }
    }
}