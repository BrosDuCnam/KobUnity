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

        public ItemSlotData data { get; private set; }
        
        [HideInInspector] public InventorySlot currentSlot;
        
        public void Refresh(ItemSlotData newData)
        {
            data = newData;
            
            ScriptableItem item = UResources.GetScriptableItemById(newData.itemId);
            
            iconImg.sprite = item.icon;
            amountTmp.text = newData.amount.ToString();
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
            InventorySlot targetSlot = GetPointedSlot(eventData);
            
            if (targetSlot == null || 
                !targetSlot.SetItem(this)) currentSlot.SetItem(this);
        }

        #endregion
        
        
        public InventorySlot GetPointedSlot(PointerEventData eventData)
        {
            // Get the game object that the mouse pointer is currently over
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (target == null)
            {
                return null;
            }

            // Try to get the InventorySlot component on the target object
            InventorySlot targetSlot = null;
            if (target.TryGetComponent(out targetSlot))
            {
                return targetSlot;
            }

            // Try to get the ItemSlot component in the children of the target object
            var itemSlot = target.GetComponentInChildren<ItemSlot>();
            if (itemSlot!= null)
            {
                return itemSlot.currentSlot;
            }

            // Try to get the InventorySlot component in the parent of the target object
            targetSlot = target.GetComponentInParent<InventorySlot>();
            if (targetSlot != null)
            {
                return targetSlot;
            }

            return null;
        }



    }
}