using System.Collections.Generic;
using Network.Data;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Components.UI.Game.Inventory
{
    
    public class BaseInventory : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private List<InventorySlot> prefabSlots; // if null or empty - will be generated
        
        public void Refresh(Data.Inventory items)
        {
            if (prefabSlots != null && prefabSlots.Count > 0)
            {
                prefabSlots.ForEach(x => x.gameObject.SetActive(true));
                
                for (int i = 0; i < prefabSlots.Count; i++)
                {
                    prefabSlots[i].slotIndex = i;
                    prefabSlots[i].currentInventory = this;
                    
                    if (i < items.items.Count)
                    {
                        prefabSlots[i].Refresh(items.items[i]);
                    }
                    else
                    {
                        prefabSlots[i].Refresh(Data.ItemSlot.Void);
                    }
                }
            }
            else
            {
                int index = 0;

                var result = UIPooling.Pool<InventorySlot, Data.ItemSlot>(items.items, slotPrefab, transform);

                result.activeItems.ForEach(x =>
                {
                    x.slotIndex = index++;
                    x.currentInventory = this;
                });
            }
        }
        
        #region Network
        
        private void OnEnable()
        {
            inventoryData.OnValueChanged.AddListener(OnValueChanged);
            
            inventoryData.OnNetworkSpawned.AddListener(() =>
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    List<Data.ItemSlot> test = new();
                    for (int i = 0; i < 64; i++)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            test.Add(Data.ItemSlot.Void);
                        }
                        else
                        {
                            test.Add(new Data.ItemSlot()
                            {
                                id = 7385,
                                amount = Random.Range(0, 99)
                            });
                        }
                    }
                    
                    inventoryData.SetInventory(test.ToArray());
                }
                
                Refresh(inventoryData.Value);
            });
        }

        private void OnValueChanged(Data.Inventory data)
        {
            Refresh(data);
        }

        #endregion
        
        public void SetItem(int index, Data.ItemSlot item)
        {
            if (index < 0 || index >= inventoryData.Value.items.Count)
            {
                Debug.LogError($"Index {index} is out of range");
                return;
            }
            
            inventoryData.SetItem(index, item);
        }
        
        public bool AddItem(Data.ItemSlot item)
        {
            int index = inventoryData.Value.items.FindIndex(x => x.IsVoid);
            if (index == -1)
            {
                return false;
            }
            
            SetItem(index, item);
            return true;
        }
    }
}