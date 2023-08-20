using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Utils
{
    public class UIPooling<TV, TK>
        where TV : UIBehaviour<TK>
        where TK : struct
    {
        
        public class PoolingResult
        {
            public List<TV> disabledItems = new();
            public List<TV> newItems = new();
            public List<TV> activeItems = new();
        }
        
        // This method refreshes the items in the pool based on the given data.
        public PoolingResult Refresh(List<TK> data, GameObject prefab, Transform container)
        {
            List<TV> items = container.GetComponentsInChildren<MonoBehaviour>().OfType<TV>().ToList();
            
            PoolingResult result = new();

            // Loop through the data and update the items in the pool.
            for (int i = 0; i < Math.Max(items.Count, data.Count); i++)
            {
                // If there is data for this index, update the item in the pool.
                if (i < data.Count)
                {
                    TV tv;
                    
                    // If there is an item in the pool for this index, update it.
                    if (i < items.Count)
                    {
                        tv = items[i];
                        items[i].Refresh(data[i]);
                    }
                    // If there is no item in the pool for this index, create a new item.
                    else
                    {
                        GameObject item = Object.Instantiate(prefab, container);
                        tv = item.GetComponent<TV>();
                        tv.Refresh(data[i]);
                        
                        items.Add(item.GetComponent<TV>());
                        result.newItems.Add(items[i]);
                    }
                    
                    result.activeItems.Add(items[i]);
                }
                // If there is no data for this index, hide the item in the pool.
                else
                {
                    items[i].GetMonoBehaviour().gameObject.SetActive(false);
                    result.disabledItems.Add(items[i]);
                }
            }
            
            return result;
        }
    }

}