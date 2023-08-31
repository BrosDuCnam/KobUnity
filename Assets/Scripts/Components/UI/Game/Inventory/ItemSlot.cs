using System.Linq;
using Managers;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class ItemSlot : MonoBehaviour, UIBehaviour<ItemSlotData>
    {

        private RectTransform _rectTransform; public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                
                return _rectTransform;
            }
        }

        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI amountTmp;
        [SerializeField] public InventorySlot currentSlot;

        public bool isGrabbed { get; private set; }
        
        public ItemSlotData Data { get; private set; }

        public void Refresh(ItemSlotData newData)
        {
            if (newData.IsVoid)
            {
                iconImg.sprite = null;
                amountTmp.text = "";
                Data = ItemSlotData.Void;
                
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            
            Data = newData;

            ScriptableItem item = UResources.GetScriptableItemById(newData.id);
            
            iconImg.sprite = item.icon;
            amountTmp.text = newData.amount.ToString();
        }
        
        #region Dragging
        
        public void SetGrabbed(bool grabbed)
        {
            if (grabbed) UpdatePos();
            
            isGrabbed = grabbed;
            canvasGroup.alpha = grabbed ? 1 : 0;
        }
        
        private void Update()
        {
            if (!isGrabbed) return;
            
            UpdatePos();
        }

        private void UpdatePos()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 mousePos = EventSystem.current.currentInputModule.input.mousePosition;
            mousePos.y -= screenSize.y;
            
            RectTransform.anchoredPosition = mousePos;
        }

        #endregion
    }
}