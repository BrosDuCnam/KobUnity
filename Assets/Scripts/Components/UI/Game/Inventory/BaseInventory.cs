using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class BaseInventory : MonoBehaviour, UIBehaviour<BaseInventory.InventoryData>
    {
        public struct InventoryData
        {
            public int slotAmount;
            public Dictionary<int, ItemSlot.ItemSlotData> items;
        }
        
        [Header("Prefabs")] 
        [SerializeField] private GameObject slotPrefab;

        private List<InventorySlot> _slots = new ();

        public void Refresh(InventoryData newData)
        {
            _slots = UIPooling.Pool<InventorySlot>(newData.slotAmount, slotPrefab, transform)
                .activeItems.ConvertAll(x => x.GetComponent<InventorySlot>());
            
            // Loop into all slot to spawn item if needed
            for (int i = 0; i < _slots.Count; i++)
            {
                InventorySlot slot = _slots[i];
                slot.currentInventory = this;
                
                if (!newData.items.ContainsKey(i))
                {
                    slot.SetItem(null);
                }
                else
                {
                    slot.SetItem(newData.items[i]);
                }
            }
        }

        private void Start()
        {
            // Test data
            InventoryData test = new InventoryData()
            {
                slotAmount = 54,
                items = new Dictionary<int, ItemSlot.ItemSlotData>()
                {
                    {0, new ItemSlot.ItemSlotData() {amount = 1, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {1, new ItemSlot.ItemSlotData() {amount = 59, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {2, new ItemSlot.ItemSlotData() {amount = 32, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {3, new ItemSlot.ItemSlotData() {amount = 48, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {4, new ItemSlot.ItemSlotData() {amount = 68, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {5, new ItemSlot.ItemSlotData() {amount = 05, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {6, new ItemSlot.ItemSlotData() {amount = 64, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}},
                    {7, new ItemSlot.ItemSlotData() {amount = 99, itemId = "7385fd06-8df4-4354-964d-2806daff3e33"}}
                }
            };
            
            Refresh(test);
        }
    }
}