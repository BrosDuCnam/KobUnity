using Managers;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, UIBehaviour<ItemSlot.ItemSlotData>
    {
        public struct ItemSlotData
        {
            public int amount;
            public string itemId;
        }
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
        [SerializeField] private CanvasGroup borders;
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI amountTmp;

        [HideInInspector] public InventorySlot currentSlot;
        public BaseInventory currentInventory => currentSlot.currentInventory;
        
        public void Refresh(ItemSlotData data)
        {
            ScriptableItem item = UResources.GetScriptableItemById(data.itemId);
            
            iconImg.sprite = item.icon;
            amountTmp.text = data.amount.ToString();
        }

        public void SetSlot(InventorySlot slot)
        {
            currentSlot = slot;
            transform.SetParent(slot.transform);
            RectTransform.anchoredPosition = Vector2.zero;
        }
        
        #region Dragging
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(MReferenceUI.Instance.dragRect);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            InventorySlot targetSlot = GetPointedSlot(eventData) ?? currentSlot; // If the target slot is null, the item is dropped on the same slot
            
            SetSlot(targetSlot);
        }

        #endregion
        
        public InventorySlot GetPointedSlot(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == null)
            {
                Debug.Log("Drop target is null");
                return null;
            }

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() == null)
            {
                Debug.Log("Drop target is not a slot");
                return null;
            }
            
            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();

            if (targetSlot.IsAvailable())
            {
                return targetSlot;
            }
            else
            {
                Debug.Log("Drop target is not available");
                return null;
            }
        }
    }
}