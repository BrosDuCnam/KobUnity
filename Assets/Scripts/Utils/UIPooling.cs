using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public static class UIPooling
    {
        
        public struct PoolingResult
        {
            public List<GameObject> disabledItems;
            public List<GameObject> newItems;
            public List<GameObject> activeItems; // Contains both new and old items.
        }
        
        // This method refreshes the items in the pool based on the given data.
        public static PoolingResult Pool<TV, TK>(IEnumerable<TK> data, GameObject prefab, Transform container) where TV : UIBehaviour<TK> where TK : struct
        {
            // Can be a problem to take all children, not only direct children.
            List<TV> items = container.GetComponentsInChildren<MonoBehaviour>(true).OfType<TV>().ToList();
            
            PoolingResult result = new() 
            {
                disabledItems = new List<GameObject>(),
                newItems = new List<GameObject>(),
                activeItems = new List<GameObject>()
            };

            // Loop through the data and update the items in the pool.
            for (int i = 0; i < Math.Max(items.Count, data.Count()); i++)
            {
                // If there is data for this index, update the item in the pool.
                if (i < data.Count())
                {
                    TV tv;
                    
                    // If there is an item in the pool for this index, update it.
                    if (i < items.Count)
                    {
                        tv = items[i];
                        tv.Refresh(data.ElementAt(i));
                    }
                    // If there is no item in the pool for this index, create a new item.
                    else
                    {
                        GameObject item = Object.Instantiate(prefab, container);
                        
                        tv = item.GetComponent<TV>();
                        tv.Refresh(data.ElementAt(i));
                        
                        items.Add(item.GetComponent<TV>());
                        result.newItems.Add(item);
                    }
                    
                    result.activeItems.Add(items[i].GetMonoBehaviour().gameObject);
                }
                // If there is no data for this index, hide the item in the pool.
                else
                {
                    items[i].GetMonoBehaviour().gameObject.SetActive(false);
                    result.disabledItems.Add(items[i].GetMonoBehaviour().gameObject);
                }
            }
            
            return result;
        }
        
        // Method to pool any UI, not only UIBehaviours.
        public static PoolingResult Pool<TK>(int amount, GameObject prefab, Transform container) where TK : MonoBehaviour
        {
            List<GameObject> items = container.GetComponentsInChildren<MonoBehaviour>(true).OfType<TK>().ToList().ConvertAll(x => x.gameObject);
            
            PoolingResult result = new ()
            {
                disabledItems = new List<GameObject>(),
                newItems = new List<GameObject>(),
                activeItems = new List<GameObject>()
            };
            
            // Loop through the data and update the items in the pool.
            for (int i = 0; i < Math.Max(items.Count, amount); i++)
            {
                // If there is data for this index, update the item in the pool.
                if (i < amount)
                {
                    GameObject item;
                    
                    // If there is an item in the pool for this index, update it.
                    if (i < items.Count)
                    {
                        item = items[i];
                    }
                    // If there is no item in the pool for this index, create a new item.
                    else
                    {
                        item = Object.Instantiate(prefab, container);
                        items.Add(item);
                        
                        result.newItems.Add(item);
                    }
                    
                    item.SetActive(true);
                    result.activeItems.Add(item);
                }
                // If there is no data for this index, hide the item in the pool.
                else
                {
                    items[i].SetActive(false);
                    result.disabledItems.Add(items[i]);
                }
            }
            
            return result;
        }
    }

}