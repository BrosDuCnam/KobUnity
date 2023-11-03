using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "New Scriptable Item", menuName = "Scriptable/Scriptable Item")]
    public class ScriptableItem : ScriptableObject
    {
        [SerializeField] public ScriptableItem parent;
        [SerializeField] public int id;
        
        [SerializeField] public string itemName;
        [SerializeField] public Sprite icon;
        [SerializeField] public int maxStack = 99;
        [SerializeField] public ConsumablesData consumablesData;
        [SerializeField] public CraftData craftData;
        
        [Serializable]
        public struct ConsumablesData
        {
            public int health;
            public int thirst;
            public int hunger;
        }

        [Serializable]
        public struct CraftData
        {
            public List<int> ids;
            public bool ordered;
            public int number;
        }
        
        public void OnValidate()
        {
            if (id == 0)
            {
                id = Guid.NewGuid().GetHashCode();
            }

            if (craftData.number < 1)
            {
                craftData.number = 1;
            }

            while (craftData.ids.Count > 9) { craftData.ids.RemoveAt(0); }

            if (!craftData.ordered)
            {
                craftData.ids.Sort();
            }
        }

        public string GetCraftRecipe()
        {
            if (!craftData.ordered)
            {
                while (craftData.ids.Count > 0 && craftData.ids[0] <= 0)
                {
                    craftData.ids.RemoveAt(0);
                }
            }

            string recipe = "";
            foreach (int id in craftData.ids) { recipe += id.ToString() + ";"; }

            return recipe;
        }

        public int[] GetCraftResult()
        {
            return new int[2] { id, craftData.number };
        }
    }
}