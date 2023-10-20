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
    public class Craft : MonoBehaviour
    {
        [SerializeField] private InventorySlot prefabCraftSlot;
        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private List<InventorySlot> prefabSlots;
        private int prefabSlotsLength = -1;

        /* craft data, [id, amount] */
        private List<string> craftRecipes;
        private List<int[]> craftResult;
        private int craftLength = -1;

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
            /* ask if can detect change from a InventorySlot */

            List<int> unorderedIds = new List<int>();
            List<int> orderedIds = new List<int>();

            for (int i = 0; i < prefabSlotsLength; i++)
            {
                int id = prefabSlots[i].currentItem.Data.id;
                
                if (id != -1)
                {
                    unorderedIds.Add(id);
                }
                orderedIds.Add(id);
            }

            unorderedIds.Sort();
            /* need to remove from orderedIds all things not useful */

            string unorderedData = "";
            string orderedData = "";

            for (int i = 0; i < unorderedIds.Count; i++) { unorderedData += unorderedIds[i].ToString(); }
            for (int i = 0; i < orderedIds.Count; i++) { orderedData += orderedIds[i].ToString(); }

            // check existing crafting recipes
            bool canCraft = false;
            for (var i = 0; i < craftLength; i++)
            {
                string recipe = craftRecipes[i];
                if (recipe == unorderedData || recipe == orderedData)
                {
                    int[] result = craftResult[i];
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

            if (!canCraft) // if no crafting recipe match, empty crafting slot
            {
                prefabCraftSlot.Refresh(Data.ItemSlot.Void);
            }
        }

        public void Load()
        {
            /* load all crafting recipes */
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
        }
    }
}