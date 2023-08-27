using UnityEngine;

namespace Managers
{
    public class MReferenceUI : MonoBehaviour
    {
        #region Singleton

        public static MReferenceUI Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion
        
        [Header("References")]
        [SerializeField] public RectTransform dragRect;
    }
}