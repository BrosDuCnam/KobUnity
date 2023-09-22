using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Managers
{
    public class MUI : MonoBehaviour
    {
        
        #region Singleton
        
        public static MUI Instance { get; private set; }
        
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
        
        [HideInInspector] public GameObject currentSelection;
        [HideInInspector] public UnityEvent<GameObject> onHoverButton = new UnityEvent<GameObject>();

        protected void Start()
        {
            // Set window size to 500x500
            Screen.SetResolution(1600/2, 900/2, false);
            
        }

        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != currentSelection)
            {
                currentSelection = selected;
                onHoverButton.Invoke(selected);
            }
        }
    }
}