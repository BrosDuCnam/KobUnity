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
        
        public List<TV> items;
        
        // This method refreshes the items in the pool based on the given data.
        public void Refresh(List<TK> data, GameObject prefab, Transform container, Action<TV> onActive = null)
        {
            if (items == null) items = container.GetComponentsInChildren<TV>().ToList();

            // Loop through the data and update the items in the pool.
            for (int i = 0; i < Math.Max(items.Count, data.Count); i++)
            {
                // If there is data for this index, update the item in the pool.
                if (i < data.Count)
                {
                    // If there is an item in the pool for this index, update it.
                    if (i < items.Count)
                    {
                        items[i].Refresh(data[i]);
                    }
                    // If there is no item in the pool for this index, create a new item.
                    else
                    {
                        GameObject item = Object.Instantiate(prefab, container);
                        TV tv = item.GetComponent<TV>();
                        tv.Refresh(data[i]);
                        
                        onActive?.Invoke(tv);
                        
                        items.Add(item.GetComponent<TV>());
                    }
                }
                // If there is no data for this index, hide the item in the pool.
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }
        }
    }

}