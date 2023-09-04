using System.Collections.Generic;
using System.Linq;
using Components.Data;
using Unity.Netcode;
using UnityEngine;

namespace Network.Data
{
    public class InventoryData : NetworkData<Inventory>
    {
        private NetworkList<ItemSlot> _items = null;

        private void Awake()
        {
            _items = new NetworkList<ItemSlot>();
        }
        
        public override void OnNetworkSpawn()
        {
            _items.OnListChanged += OnServerValueChanged;
            
            base.OnNetworkSpawn();
        }

        private void OnServerValueChanged(NetworkListEvent<ItemSlot> change)
        {
            if (change.Index < 0 || change.Index >= _items.Count) return;
            
            Debug.Log("[DEBUG/OnServerValueChanged]");
            
            List<ItemSlot> items = new List<ItemSlot>();
            foreach (var item in _items)
            {
                items.Add(item);
            }
            
            items[change.Index] = change.Value;
            
            Value = new Inventory() { items = items };
        }
        
        public override Inventory GetValue()
        {
            List<ItemSlot> items = new List<ItemSlot>();
            foreach (var item in _items)
            {
                items.Add(item);
            }
            
            return new Inventory() { items = items };
        }
        
        #region SetItem
        
        public void SetItem(int index, ItemSlot item)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _SetItem(index, item);
            }
            else
            {
                SetItemServerRpc(index, item);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetItemServerRpc(int index, ItemSlot item)
        {
            _SetItem(index, item);
        }
        
        private void _SetItem(int index, ItemSlot item)
        {
            Debug.Log("[DEBUG/SetItem]: " + index + " " + item);
            
            _items[index] = item;
        }
        
        #endregion

        #region SetInventory
        
        public void SetInventory(ItemSlot[] items)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _SetInventory(items);
            }
            else
            {
                SetInventoryServerRpc(items);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetInventoryServerRpc(ItemSlot[] items)
        {
            _SetInventory(items);
        }
        
        private void _SetInventory(ItemSlot[] items)
        {
            _items.Clear();
            foreach (var item in items)
            {
                _items.Add(item);
            }
        }
        
        #endregion
    }
}