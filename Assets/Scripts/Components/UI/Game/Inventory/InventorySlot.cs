using System;
using Components.Data;
using DG.Tweening;
using Managers;
using Scriptable;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : MonoBehaviour, UIBehaviour<Data.ItemSlot>, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private CanvasGroup borders;
        [SerializeField] public ItemSlot currentItem;
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
                Data.ItemSlot rest = TryPutItem(MReferenceUI.Instance.grabbedItem.Data);
                if (rest.IsVoid)
                {
                    MReferenceUI.Instance.grabbedItem.Refresh(Data.ItemSlot.Void);
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
                    SetItem(Data.ItemSlot.Void);
                }
            }
        }

        public void OnRightClick()
        {
            if (MReferenceUI.Instance.grabbedItem.isGrabbed)
            {
                (Data.ItemSlot left, Data.ItemSlot right) = MReferenceUI.Instance.grabbedItem.Data.Split(1);
                
                Data.ItemSlot rest = TryPutItem(left);
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
                    (Data.ItemSlot left, Data.ItemSlot right) = GetData().Split(); // Split in half
                    SetItem(left);
                    
                    MReferenceUI.Instance.grabbedItem.Refresh(right);
                    MReferenceUI.Instance.grabbedItem.SetGrabbed(true);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(borders.DOFade(1, 0.15f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(borders.DOFade(0, 0.15f));
        }

        #endregion
        
        public void Refresh(Data.ItemSlot @new)
        {
            SetItem(@new, false);
        }
        
        public bool HasItem()
        {
            return currentItem.gameObject.activeSelf;
        }

        public Data.ItemSlot GetData()
        {
            if (HasItem()) return currentItem.Data;
            return Data.ItemSlot.Void;
        }
        
        public void SetItem(Data.ItemSlot item, bool notify = true)
        {
            if (!HasItem() && item.IsVoid) {
                return;
            }

            currentItem.Refresh(item);

            if (notify) currentInventory.SetItem(slotIndex, item);
        }
        

        public int HowMuchCanFit(Data.ItemSlot item)
        {
            if (!HasItem()) return item.amount;

            if (GetData().id != item.id) return 0;

            int maxAmount = UResources.GetScriptableItemById(item.id).maxStack;
            int sum = GetData().amount + item.amount;
            
            return sum > maxAmount ? maxAmount - GetData().amount : item.amount;
        }
        
        public Data.ItemSlot TryPutItem(Data.ItemSlot item)
        {
            if (item.IsVoid)
            {
                SetItem(Data.ItemSlot.Void);
                return Data.ItemSlot.Void;
            }
            
            int howMuchCanFit = HowMuchCanFit(item);
            if (howMuchCanFit == 0) return item;
            
            Data.ItemSlot @new = new Data.ItemSlot()
            {
                id = item.id,
                amount = GetData().amount + howMuchCanFit
            };
            Data.ItemSlot rest = item.Add(-howMuchCanFit);

            SetItem(@new);
            return rest;
        }
    }
}