using System;
using DG.Tweening;
using Managers;
using Scriptable;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : MonoBehaviour, UIBehaviour<global::Data.ItemSlot>, IPointerClickHandler
    {
        [Header("References")]
        [SerializeField] private CanvasGroup background;
        [SerializeField] public ItemSlot currentItem;
        
        [Space]
        
        [SerializeField] public BaseInventory currentInventory;

        public int slotIndex;
        private Sequence _sequence;
        
        #region Button Implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }
        
        public void OnLeftClick()
        {
            
            if (MReferenceUI.Instance.grabbedItem.isGrabbed)
            {
                global::Data.ItemSlot rest = TryPutItem(MReferenceUI.Instance.grabbedItem.Data);
                if (rest.IsVoid)
                {
                    MReferenceUI.Instance.grabbedItem.Refresh(global::Data.ItemSlot.Void);
                    MReferenceUI.Instance.grabbedItem.SetGrabbed(false);
                }
                else MReferenceUI.Instance.grabbedItem.Refresh(rest);
            }
            else
            {
                if (HasItem())
                {
                    MReferenceUI.Instance.grabbedItem.Refresh(GetData());
                    MReferenceUI.Instance.grabbedItem.SetGrabbed(true);
                    SetItem(global::Data.ItemSlot.Void);
                }
            }
        }

        public void OnRightClick()
        {
            if (MReferenceUI.Instance.grabbedItem.isGrabbed)
            {
                (global::Data.ItemSlot left, global::Data.ItemSlot right) = MReferenceUI.Instance.grabbedItem.Data.Split(1);
                
                global::Data.ItemSlot rest = TryPutItem(left);
                if (rest.IsVoid) // If all of the item was put in the slot
                {
                    MReferenceUI.Instance.grabbedItem.Refresh(right);
                    if (right.IsVoid)
                    {
                        MReferenceUI.Instance.grabbedItem.SetGrabbed(false);
                    }
                }
            }
            else
            {
                if (HasItem())
                {
                    (global::Data.ItemSlot left, global::Data.ItemSlot right) = GetData().Split(); // Split in half
                    SetItem(left);
                    
                    MReferenceUI.Instance.grabbedItem.Refresh(right);
                    MReferenceUI.Instance.grabbedItem.SetGrabbed(true);
                }
            }
        }

        #endregion
        
        public void Refresh(global::Data.ItemSlot newItem)
        {
            SetItem(newItem, false);
            
            if (HasItem())
            {
                background.alpha = 0;
            }
            else
            {
                background.alpha = 1;
            }
        }
        
        public bool HasItem()
        {
            return currentItem.gameObject.activeSelf;
        }

        public global::Data.ItemSlot GetData()
        {
            if (HasItem()) return currentItem.Data;
            return global::Data.ItemSlot.Void;
        }
        
        public void SetItem(global::Data.ItemSlot item, bool notify = true)
        {
            if (!HasItem() && item.IsVoid) {
                return;
            }

            currentItem.Refresh(item);

            if (notify) currentInventory.SetItem(slotIndex, item);
        }
        

        public int HowMuchCanFit(global::Data.ItemSlot item)
        {
            if (!HasItem()) return item.amount;

            if (GetData().id != item.id) return 0;

            int maxAmount = UResources.GetScriptableItemById(item.id).maxStack;
            int sum = GetData().amount + item.amount;
            
            return sum > maxAmount ? maxAmount - GetData().amount : item.amount;
        }
        
        public global::Data.ItemSlot TryPutItem(global::Data.ItemSlot item)
        {
            if (item.IsVoid)
            {
                SetItem(global::Data.ItemSlot.Void);
                return global::Data.ItemSlot.Void;
            }
            
            int howMuchCanFit = HowMuchCanFit(item);
            if (howMuchCanFit == 0) return item;
            
            global::Data.ItemSlot @new = new global::Data.ItemSlot()
            {
                id = item.id,
                amount = GetData().amount + howMuchCanFit
            };
            global::Data.ItemSlot rest = item.Add(-howMuchCanFit);

            SetItem(@new);
            return rest;
        }
    }
}