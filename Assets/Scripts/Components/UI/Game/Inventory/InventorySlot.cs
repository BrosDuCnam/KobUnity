using Managers;
using Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : CustomButton, UIBehaviour<InventorySlot.ItemSlotData>, IPointerClickHandler
    {
        public struct ItemSlotData
        {
            public int amount;
            public string itemId;
            
            public ItemSlotData Add(int amountAdded)
            {
                return new ItemSlotData()
                {
                    amount = amount + amountAdded,
                    itemId = itemId
                };
            }
            public (ItemSlotData, ItemSlotData) Split(int amountSplit = -1)
            {
                int leftAmount = amountSplit == -1 ? amount / 2 : amountSplit;
                int rightAmount = amount - leftAmount;
                
                return (new ItemSlotData()
                {
                    amount = leftAmount,
                    itemId = itemId
                }, new ItemSlotData()
                {
                    amount = rightAmount,
                    itemId = itemId
                });
            }

            public bool IsVoid => amount == 0 || string.IsNullOrEmpty(itemId);
            public static ItemSlotData Void => new ItemSlotData()
            {
                amount = 0,
                itemId = ""
            };
            
            public static bool operator ==(ItemSlotData a, ItemSlotData b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(ItemSlotData a, ItemSlotData b)
            {
                return !(a == b);
            }
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
        
        public BaseInventory currentInventory;
        public ItemSlot currentItem;

        #region Button Implementation

        public void OnLeftClick()
        {
            if (MReferenceUI.Instance.grabbedItem.isGrabbed)
            {
                ItemSlotData rest = TryPutItem(MReferenceUI.Instance.grabbedItem.Data);
                if (rest.IsVoid)
                {
                    MReferenceUI.Instance.grabbedItem.Refresh(ItemSlotData.Void);
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
                    SetItem(ItemSlotData.Void);
                }
            }
        }

        public void OnRightClick()
        {
            if (MReferenceUI.Instance.grabbedItem.isGrabbed)
            {
                (ItemSlotData left, ItemSlotData right) = MReferenceUI.Instance.grabbedItem.Data.Split(1);
                
                ItemSlotData rest = TryPutItem(left);
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
                    (ItemSlotData left, ItemSlotData right) = GetData().Split(); // Split in half
                    SetItem(left);
                    
                    MReferenceUI.Instance.grabbedItem.Refresh(right);
                    MReferenceUI.Instance.grabbedItem.SetGrabbed(true);
                }
            }
        }

        #endregion
        
        public bool HasItem()
        {
            return currentItem.gameObject.activeSelf;
        }

        public ItemSlotData GetData()
        {
            if (HasItem()) return currentItem.Data;
            return new ItemSlotData()
            {
                itemId = "",
                amount = 0
            };
        }
        
        public void SetItem(ItemSlotData item)
        {
            if (!HasItem() && item.IsVoid) {
                return;
            }

            if (!item.IsVoid)
            {
                currentItem.Refresh(item);
                UpdateItemPos();
                
                currentItem.gameObject.SetActive(true);
            }
            else currentItem.gameObject.SetActive(false);
        }

        private void UpdateItemPos()
        {
            currentItem.RectTransform.anchoredPosition = Vector2.zero;
        }

        public int HowMuchCanFit(ItemSlotData item)
        {
            if (!HasItem()) return item.amount;

            if (GetData().itemId != item.itemId) return 0;

            int maxAmount = UResources.GetScriptableItemById(item.itemId).maxStack;
            int sum = GetData().amount + item.amount;
            
            return sum > maxAmount ? maxAmount - GetData().amount : item.amount;
        }
        
        public ItemSlotData TryPutItem(ItemSlotData item)
        {
            if (item.IsVoid)
            {
                SetItem(ItemSlotData.Void);
                return ItemSlotData.Void;
            }
            
            int howMuchCanFit = HowMuchCanFit(item);
            if (howMuchCanFit == 0) return item;
            
            ItemSlotData newData = new ItemSlotData()
            {
                itemId = item.itemId,
                amount = GetData().amount + howMuchCanFit
            };
            ItemSlotData rest = item.Add(-howMuchCanFit);

            SetItem(newData);
            return rest;
        }

        public void Refresh(ItemSlotData newData)
        {
            SetItem(newData);
        }

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
    }
}