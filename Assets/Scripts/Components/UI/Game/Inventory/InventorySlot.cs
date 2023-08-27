using UnityEngine;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public BaseInventory currentInventory;
        public ItemSlot currentItem;
        
        public bool IsAvailable()
        {
            if (currentItem != null) return false;
            if (currentInventory == null) return false;
            
            return true;
        }
    }
}