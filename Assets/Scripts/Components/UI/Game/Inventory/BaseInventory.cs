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
        
        [Header("References")]
        [SerializeField] private RectTransform contentRect;
        
        [Header("Prefabs")] 
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private GameObject itemPrefab;

        private List<InventorySlot> _slots = new List<InventorySlot>();
        private List<ItemSlot> _items = new List<ItemSlot>();

        public void Refresh(InventoryData newData)
        {
            _slots = UIPooling.Pool<InventorySlot>(newData.slotAmount, slotPrefab, transform)
                .activeItems.ConvertAll(x => x.GetComponent<InventorySlot>());
            
            _slots.ForEach(x => x.currentInventory = this);

            // Make a list to conserve the order of the items.
            KeyValuePair<int, ItemSlot.ItemSlotData>[] tempItems = newData.items.ToArray(); 
            
            _items = UIPooling.Pool<ItemSlot, ItemSlot.ItemSlotData>(tempItems.Select(x => x.Value), itemPrefab, contentRect) 
                .activeItems.ConvertAll(x => x.GetComponent<ItemSlot>());

            int i = 0;
            foreach (KeyValuePair<int, ItemSlot.ItemSlotData> kvpItem in tempItems)
            {
                InventorySlot slot = _slots[i];
                ItemSlot item = _items[i];
                
                slot.SetItem(item);
                
                i++;
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