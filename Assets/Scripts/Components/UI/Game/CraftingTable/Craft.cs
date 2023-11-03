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
    public class Craft : BaseInventory
    {
        [SerializeField] private InventorySlot prefabCraftSlot;
        //TEST [SerializeField] private List<InventorySlot> prefabSlots;
        private int prefabSlotsLength = -1;

        /* craft data, [id, amount] */
        private List<string> craftRecipes = new List<string>();
        private List<int[]> craftResult = new List<int[]>();
        private int craftLength = -1;
        
        // all legal zone to craft in (all rect with any dimension in 3x3 area) (need to be squares)
        private static List<List<int[]>> min_rect = new List<List<int[]>>
        {
            new List<int[]> { // 1
                   new int[] { 0 },
                   new int[] { 1 },
                   new int[] { 2 },
                   new int[] { 3 },
                   new int[] { 4 },
                   new int[] { 5 },
                   new int[] { 6 },
                   new int[] { 7 },
                   new int[] { 8 },
                   },
            new List<int[]> { },// 2
                //new int[] { 0, 1 }, new int[] { 1, 2 }, new int[] { 3, 4 }, new int[] { 4, 5 }, new int[] { 6, 7 }, new int[] { 7, 8 }, new int[] { 0, 3 }, new int[] { 3, 6 }, new int[] { 1, 4 }, new int[] { 4, 7 }, new int[] { 2, 5 }, new int[] { 5, 8 }, },
            new List<int[]> { },// 3
                //new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }, new int[] { 6, 7, 8 }, new int[] { 0, 3, 6 }, new int[] { 1, 4, 7 }, new int[] { 2, 5, 8 }, },
            new List<int[]> { // 4
                new int[] { 0, 1, 3, 4 },
                new int[] { 1, 2, 4, 5 },
                new int[] { 3, 4, 6, 7 },
                new int[] { 4, 5, 7, 8 },
            },
            new List<int[]> { }, // 5
            new List<int[]> { },// 6
                //new int[] { 0, 1, 3, 4, 6, 7 }, new int[] { 1, 2, 4, 5, 7, 8 }, new int[] { 0, 1, 2, 3, 4, 5 }, new int[] { 3, 4, 5, 6, 7, 8 }, },
            new List<int[]> { }, // 7
            new List<int[]> { }, // 8
            new List<int[]> {  // 9
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }
            },
        };

        public void Refresh()
        {
            if (prefabSlotsLength == -1)
            {
                prefabSlotsLength = prefabSlots.Count;
            }
            if (craftLength == -1)
            {
                craftLength = craftRecipes.Count;
            }

            /* compare previous prefabSlots data with current One
             * if different and prefabCraftSlots had something
             * then remove 1 from all prefabSlots
             */

            List<int> unorderedIds = new List<int>(); // only item ordered by ids
            List<int> orderedIds = new List<int>(); // order in item with minimal rectangle form
            int count = 0; // count number of item in crafting ; use for ordered Ids
            bool canCraft = false;

            for (int i = 0; i < prefabSlotsLength; i++)
            {
                int id = prefabSlots[i].currentItem.Data.id;
                Debug.Log("TEST CRAFT: " + id.ToString());
                if (id > 0)
                {
                    unorderedIds.Add(id);
                    count += 1;
                }
                orderedIds.Add(id);
            }

            Debug.Log("Craft Refresh: " + count.ToString());

            unorderedIds.Sort();
            // remove unuseful things in orderedIds to get minimal rectangle
            if (count > 0)
            {
                int count_in_rect;
                bool rect_found = false;

                int i = count - 1;
                while (i < min_rect.Count && !rect_found)
                {
                    int j = 0;
                    while (j < min_rect[i].Count && !rect_found)
                    {
                        // count items inside of current rect
                        count_in_rect = 0;
                        foreach (int index in min_rect[i][j])
                        {
                            if (orderedIds[index] != -1)
                            {
                                count_in_rect += 1;
                            }
                        }

                        if (count == count_in_rect)
                        {
                            List<int> new_orderedIds = new List<int>();
                            foreach (int index in min_rect[i][j])
                            {
                                new_orderedIds.Add(orderedIds[index]);
                            }

                            orderedIds = new_orderedIds;
                            rect_found = true;
                        }

                        j++;
                    }

                    i++;
                }

                /* need to remove from orderedIds all things not useful */
                string unorderedData = "";
                string orderedData = "";

                foreach (int id in unorderedIds) { unorderedData += id.ToString() + ";"; }
                foreach (int id in orderedIds) { orderedData += id.ToString() + ";"; }

                //TEST
                Debug.Log("Unordered: " + unorderedData);
                Debug.Log("Ordered: " + orderedData);

                // check existing crafting recipes
                for (int k = 0; k < craftLength; k++)
                {
                    string recipe = craftRecipes[k];
                    Debug.Log("TEST Recipe: " + recipe);
                    if (recipe == unorderedData || recipe == orderedData)
                    {
                        int[] result = craftResult[k];

                        Debug.Log("CAN CRAFT: " + result.ToString());
                        Components.Data.ItemSlot item = new Components.Data.ItemSlot()
                        {
                            id = result[0],
                            amount = result[1]
                        };

                        prefabCraftSlot.Refresh(item);
                        canCraft = true;
                        break;
                    }
                }
            }

            if (!canCraft) // if no crafting recipe match, empty crafting slot
            {
                prefabCraftSlot.Refresh(Data.ItemSlot.Void);
            }
        }
        
        public override void Refresh(Data.Inventory items)
        {
            base.Refresh(items);

            Refresh();
        }

        //TEST @Mathias - ask when load is appropriate
        private void Start()
        {
            Load();
        }
        //TEST @Mathias - ask if can detect change from an InventorySlot
        //private void Update()
        //{
        //    Refresh();
        //}

        public void Load()
        {
            /* load all crafting recipes */
            List<Scriptable.ScriptableItem> ressources = UResources.GetAllScriptableItems();
            foreach (Scriptable.ScriptableItem item in ressources)
            {
                string recipe = item.GetCraftRecipe();
                Debug.Log("RECIPE: " + recipe);
                if (recipe != "")
                {
                    craftRecipes.Add(recipe);
                    craftResult.Add(item.GetCraftResult());
                }
            }

            craftLength = craftRecipes.Count;
            Debug.Log("LOAD IN: " + craftLength.ToString());
        }
    }
}