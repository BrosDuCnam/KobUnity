using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public static class UIPooling
    {
        // This method refreshes the items in the pool based on the given data.
        public static void Refresh<TV, TK>(List<TK> data, GameObject prefab, Transform container)
            where TV : UIBehaviour<TK>
            where TK : struct
        {
            // Get all the items in the container.
            List<TV> items = new List<TV>();
            items.AddRange(container.GetComponentsInChildren<TV>());

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
                        item.GetComponent<TV>().Refresh(data[i]);
                    }
                }
                // If there is no data for this index, destroy the item in the pool.
                else
                {
                    Object.Destroy(items[i].gameObject);
                }
            }
        }
    }

}