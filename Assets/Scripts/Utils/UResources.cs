using System.Collections.Generic;
using Components.Building;
using UnityEngine;

namespace Utils
{
    public static class UResources
    {
        // Resources paths
        public const string ITEM_SCRIPTABLE_PATH = "Contents/Items/";
        public const string BLOCK_PREFAB_PATH = "Contents/Blocks/";
        
        // Cached resources
        private static readonly List<Scriptable.ScriptableItem> ScriptableItems = new();
        private static readonly List<Block> Blocks = new();
        
        // Getters
        public static Scriptable.ScriptableItem GetScriptableItemById(int id)
        {
            if (ScriptableItems.Count == 0)
            {
                ScriptableItems.AddRange(Resources.LoadAll<Scriptable.ScriptableItem>(ITEM_SCRIPTABLE_PATH));
            }

            return ScriptableItems.Find(x => x.id == id);
        }
        
        public static Block GetBlockById(int id)
        {
            if (Blocks.Count == 0)
            {
                Blocks.AddRange(Resources.LoadAll<Block>(BLOCK_PREFAB_PATH));
            }

            return Blocks.Find(x => x.id == id);
        }
    }
}