using Scriptable;
using UnityEngine;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public BaseInventory currentInventory;
        public ItemSlot currentItem;

        public bool HasItem()
        {
            return currentItem.gameObject.activeSelf;
        }

        public ItemSlot.ItemSlotData GetData()
        {
            if (HasItem()) return currentItem.Data;
            return new ItemSlot.ItemSlotData()
            {
                itemId = "",
                amount = 0
            };
        }
        
        public void SetItem(ItemSlot.ItemSlotData? item)
        {
            if (!HasItem() &&!item.HasValue) {
                return;
            }

            // Check if the new item is not null and is equal to the current item's data
            if (item.HasValue && GetData().Equals(item.Value))
            {
                UpdateItemPos();
                return;
            }

            if (item.HasValue)
            {
                currentItem.Refresh(item.Value);
                UpdateItemPos();
                currentItem.gameObject.SetActive(true);
            }
            else currentItem.gameObject.SetActive(false);
        }

        private void UpdateItemPos()
        {
            currentItem.RectTransform.anchoredPosition = Vector2.zero;
        }

        public bool IsAvailable(ItemSlot.ItemSlotData item)
        {
            if (!HasItem()) return true;

            if (GetData().itemId != item.itemId) return false;

            ScriptableItem data = UResources.GetScriptableItemById(item.itemId);
            int sum = GetData().amount + item.amount;
            if (sum > data.maxStack) return false;

            return true;
        }
        
        public bool TryPutItem(ItemSlot.ItemSlotData? item)
        {
            if (!item.HasValue)
            {
                SetItem(null);
                return true;
            }
            
            if (!IsAvailable(item.Value)) return false;

            
            ItemSlot.ItemSlotData newData = new ItemSlot.ItemSlotData()
            {
                itemId = item.Value.itemId,
                amount = item.Value.amount + GetData().amount
            };

            SetItem(newData);
            return true;
        }
    }
}