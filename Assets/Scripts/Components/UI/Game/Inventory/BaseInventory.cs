using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class BaseInventory : MonoBehaviour, UIBehaviour<BaseInventory.InventoryData>
    {
        public struct InventoryData
        {
            public List<ItemSlotData> items;
        }

        
        [Header("Prefabs")] 
        [SerializeField] private GameObject slotPrefab;

        private List<InventorySlot> _slots = new ();

        public void Refresh(InventoryData newData)
        {
            UIPooling.Pool<InventorySlot, ItemSlotData>(newData.items, slotPrefab, transform);
        }

        private void Start()
        {
            // Test data
            InventoryData test = new InventoryData()
            {
                items = new List<ItemSlotData>()
                {
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 1}},
                    {new () {itemId = "", amount = 15}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 1}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 25}},
                    {new () {itemId = "", amount = 37}},
                    {new () {itemId = "", amount = 84}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 34}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 83}},
                    {new () {itemId = "", amount = 0}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 57}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 68}},
                    {new () {itemId = "", amount = 34}},
                    {new () {itemId = "", amount = 18}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 92}},
                    {new () {itemId = "7385fd06-8df4-4354-964d-2806daff3e33", amount = 64}},
                    {new () {itemId = "", amount = 28}},
                    
                }
            };
            
            Refresh(test);
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
            if (Cursor.visible != true) Cursor.visible = true;
        }
    }
}