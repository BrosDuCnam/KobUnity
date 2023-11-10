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
        private bool canGetCraft = false;
        //[SerializeField] private List<InventorySlot> prefabSlots; // already exist in BaseInventory
        private int prefabSlotsLength = -1;

        /* craft data, [id, amount] */
        private List<string> craftRecipes = new List<string>();
        private List<int[]> craftResult = new List<int[]>();
        private int craftLength = -1;
        
        // all legal zone to craft in (all possible squares with index + 1 square unit ==> 1x1, 2x2, 3x3)
        private static List<List<int[]>> squaresCraft = new List<List<int[]>>
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
            List<int> orderedIds = new List<int>(); // order in item with minimal square form
            int count = 0; // count number of item in crafting ; use for ordered Ids
            canGetCraft = false;

            // count number of slot occupied with items
            // set all id in orderedIds and only non void id in unorderedIds
            for (int i = 0; i < prefabSlotsLength; i++)
            {
                int id = prefabSlots[i].currentItem.Data.id;
                if (id > 0)
                {
                    unorderedIds.Add(id);
                    count += 1;
                }
                orderedIds.Add(id);
            }

            unorderedIds.Sort();
            // remove unuseful things in orderedIds to get minimal rectangle
            if (count > 0)
            {
                int count_in_rect;
                bool rect_found = false;

                int i = count - 1;
                while (i < squaresCraft.Count && !rect_found)
                {
                    int j = 0;
                    while (j < squaresCraft[i].Count && !rect_found)
                    {
                        // count items inside of current rect
                        count_in_rect = 0;
                        foreach (int index in squaresCraft[i][j])
                        {
                            if (orderedIds[index] > 0)
                            {
                                count_in_rect += 1;
                            }
                        }

                        if (count == count_in_rect)
                        {
                            List<int> new_orderedIds = new List<int>();
                            foreach (int index in squaresCraft[i][j])
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

                // Make item List into String for comparison
                string unorderedData = "U;";
                string orderedData = "O;";

                foreach (int id in unorderedIds) { unorderedData += id.ToString() + ";"; }
                foreach (int id in orderedIds) { orderedData += id.ToString() + ";"; }

                // check if match with an existing crafting recipes
                for (int k = 0; k < craftLength; k++)
                {
                    string recipe = craftRecipes[k];
                    if (recipe == unorderedData || recipe == orderedData)
                    {
                        // create result item
                        int[] result = craftResult[k];
                        Components.Data.ItemSlot item = new Components.Data.ItemSlot()
                        {
                            id = result[0],
                            amount = result[1]
                        };

                        // set in slot
                        prefabCraftSlot.Refresh(item);
                        canGetCraft = true;
                        break;
                    }
                }
            }

            if (!canGetCraft) // if no crafting recipe match, empty crafting slot
            {
                prefabCraftSlot.Refresh(Data.ItemSlot.Void);
            }
        }
        
        public override void Refresh(Data.Inventory items)
        {
            base.Refresh(items);

            Refresh();
        }

        public override void SetItem(int index, Data.ItemSlot item)
        {
            // if item tale from prefabCraftSlot (index = -1); must decrement all items in craft by 1; then refresh for check
            if (index == -1) // index for prefab Slot
            {
                // canGetCraft used to detect if not tried to put item in craft slot when empty (should not arrived anymore but not test yet)
                if (item.id == 0 && canGetCraft) // if slot is empty
                {
                    foreach(InventorySlot slot in prefabSlots)
                    {
                        slot.DecrementItem(true);
                    }

                    Refresh();
                }
            }
            else if (index < 0 || index >= prefabSlotsLength)
            {
                Debug.LogError($"Index {index} is out of range");
                return;
            }

            inventoryData.SetItem(index, item);
        }

        //TEST @Mathias - ask when load is appropriate
        //TEST @Mathias - ? if all operation done on server, maybe only load on server ?
        private void Start()
        {
            Load();
        }

        public void Load()
        {
            // load all crafting recipes
            // all craftRecipes have equals in craftResult
            
            List<Scriptable.ScriptableItem> ressources = UResources.GetAllScriptableItems();
            foreach (Scriptable.ScriptableItem item in ressources)
            {
                string recipe = item.GetCraftRecipe();
                if (recipe != "")
                {
                    craftRecipes.Add(recipe);
                    craftResult.Add(item.GetCraftResult());
                }
            }

            craftLength = craftRecipes.Count;
        }
    }
}