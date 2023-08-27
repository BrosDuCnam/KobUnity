using Scriptable;
using UnityEngine;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public BaseInventory currentInventory;
        public ItemSlot currentItem;
        
        public bool IsAvailable(ItemSlot.ItemSlotData item)
        {
            if (currentItem == null) return true;
            if (currentItem.data.itemId != item.itemId) return false;
            
            ScriptableItem itemData = UResources.GetScriptableItemById(item.itemId);
            if (currentItem.data.amount + item.amount > itemData.maxStack) return false;
            
            return true;
        }
        
        public bool SetItem(ItemSlot item)
        {
            if (!IsAvailable(item.data)) return false;
            item.transform.SetParent(transform);
            
            if (currentItem != null &&  
                currentItem != item && 
                currentItem.data.itemId == item.data.itemId)
            {
                ItemSlot.ItemSlotData tempData = currentItem.data;
                tempData.amount += item.data.amount;
                
                currentItem.Refresh(tempData);
                
                item.gameObject.SetActive(false);
            }
            else
            {
                item.RectTransform.anchoredPosition = Vector2.zero;
                item.currentSlot = this;
                
                currentItem = item;
            }

            return true;
        }
    }
}