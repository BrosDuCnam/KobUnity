using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Components.UI.Game.Inventory
{
    
    public class BaseInventory : NetworkBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject slotPrefab;
        
        private NetworkVariable<InventoryData> data = new ();
        
        public void Refresh(InventoryData items)
        {
            int index = 0;

            var result = UIPooling.Pool<InventorySlot, ItemSlotData>(items.items, slotPrefab, transform);
            
            result.activeItems.ForEach(x =>
            {
                x.slotIndex = index++;
                x.currentInventory = this;
            });
        }
        
        #region Network

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            data.OnValueChanged += OnServerValueChanged;

            if (IsHost)
            {
                List<ItemSlotData> dataList = new List<ItemSlotData>();
                for (int i = 0; i < 89; i++)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        dataList.Add(new ItemSlotData()
                        {
                            amount = Random.Range(1, 100),
                            id = 7385
                        });
                    }
                    else dataList.Add(ItemSlotData.Void);
                }

                data.Value = new InventoryData() { items = dataList };
            }
        }

        private void OnServerValueChanged(InventoryData previousvalue, InventoryData newvalue)
        {
            Refresh(newvalue);
        }
        
        [ServerRpc]
        public void SetSlotItemServerRpc(int slotIndex, ItemSlotData data)
        {
            this.data.Value.items[slotIndex] = data;
        }

        private void Update()
        {
            if (false) return;
        }

        #endregion
    }
}