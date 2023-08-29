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
        [SerializeField] public InventorySlot currentSlot;
        
        public ItemSlotData Data { get; private set; }
        
        public void Refresh(ItemSlotData newData)
        {
            Data = newData;
            
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
            transform.SetParent(currentSlot.transform);
    
            InventorySlot targetSlot = GetPointedSlot(eventData);
            Debug.Log("[DEBUG/ItemSlot]: targetSlot found", targetSlot);
    
            if (targetSlot != null)
            {
                if (targetSlot.TryPutItem(Data))
                {
                    currentSlot.SetItem(null);
                }
                else
                {
                    currentSlot.SetItem(Data);
                }
            }
            else
            {
                currentSlot.SetItem(Data);
            }
        }


        #endregion
        
        
        // Function to get the slot behind the cursor
        public InventorySlot GetPointedSlot(PointerEventData eventData)
        {
            // Get the game object that the mouse pointer is currently over
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (target == null)
            {
                return null;
            }

            if (target == gameObject || target.GetComponentInParent<ItemSlot>() == this) return null;
            
            // Try to get the InventorySlot component on the target object
            InventorySlot targetSlot = null;
            if (target.TryGetComponent(out targetSlot))
            {
                return targetSlot;
            }

            // Try to get the ItemSlot component in the children of the target object
            var itemSlot = target.transform.parent.GetComponent<ItemSlot>();
            if (itemSlot != null)
            {
                return itemSlot.currentSlot;
            }

            // Try to get the InventorySlot component in the parent of the target object
            targetSlot = target.transform.parent.GetComponent<InventorySlot>();
            if (targetSlot != null)
            {
                return targetSlot;
            }

            return null;
        }
    }
}