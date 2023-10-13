using System.Collections.Generic;
using Interfaces;
using Network.Data;
using SimpleJSON;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Components.UI.Game.Inventory
{
    
    public class BaseInventory : MonoBehaviour, ISavable
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private List<InventorySlot> prefabSlots; // if null or empty - will be generated
        
        public void Refresh(Data.Inventory items)
        {
            Debug.Log("REFRESH TRIGGER TEST");

            if (prefabSlots != null && prefabSlots.Count > 0)
            {
                Debug.Log("REFRESH EXISTS");

                prefabSlots.ForEach(x => x.gameObject.SetActive(true));
                
                for (int i = 0; i < prefabSlots.Count; i++)
                {
                    prefabSlots[i].slotIndex = i;
                    prefabSlots[i].currentInventory = this;
                    
                    if (i < items.items.Count)
                    {
                        Debug.Log("SET THING");
                        prefabSlots[i].Refresh(items.items[i]);
                    }
                    else
                    {
                        Debug.Log("SET VOID");
                        prefabSlots[i].Refresh(Data.ItemSlot.Void);
                    }
                }
            }
            else
            {
                Debug.Log("REFRESH NON EXISTS");
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
                /*
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
                }*/
                
                Refresh(inventoryData.Value);
            });
        }

        private void OnValueChanged(Data.Inventory data)
        {
            Refresh(data);
        }

        #endregion
        
        #region ISavable
        
        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            JSONArray items = new JSONArray();

            if (inventoryData.Value.items is { Count: > 0 })
            {
                foreach (var item in inventoryData.Value.items)
                {
                    items.Add(item.Save());
                }
            }

            json.Add("items", items);
            
            return json;
        }
        
        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("items", new JSONArray());
            
            return json;
        }
        
        public void Load(JSONObject json)
        {
            JSONArray items = json["items"].AsArray;
            
            List<Data.ItemSlot> slots = new List<Data.ItemSlot>();
            foreach (var item in items)
            {
                JSONNode node = item.Value;
                Data.ItemSlot slot = new Data.ItemSlot();
                
                slot.Load(node.AsObject);
                slots.Add(slot);
            }
            
            inventoryData.SetInventory(slots.ToArray());
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
            // TODO: check if item can be stacked with existing item
            int index = inventoryData.Value.items.FindIndex(x => x.IsVoid);
            Debug.Log("TEST: " + inventoryData.Value.items.Count.ToString() + " - " + index.ToString());

            if (index == -1)
            {
                return false;
            }
            Debug.Log("ITEM ADDED");
            SetItem(index, item);
            return true;
        }
    }
}