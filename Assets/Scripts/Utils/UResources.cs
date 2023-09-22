using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class UResources
    {
        // Resources paths
        public const string ITEM_SCRIPTABLE_PATH = "Contents/Items/";
        public const string BLOCK_SCRIPTABLE_PATH = "Contents/Blocks";
        
        
        // Cached resources
        private static readonly List<Scriptable.ScriptableItem> ScriptableItems = new();
        private static readonly List<Scriptable.BlockObject> ScriptableBlocks = new();
        
        // Getters
        public static Scriptable.ScriptableItem GetScriptableItemById(int id)
        {
            if (ScriptableItems.Count == 0)
            {
                ScriptableItems.AddRange(Resources.LoadAll<Scriptable.ScriptableItem>(ITEM_SCRIPTABLE_PATH));
            }

            return ScriptableItems.Find(x => x.id == id);
        }
        public static Scriptable.ScriptableItem GetScriptableBlockById(int id)
        {
            if (ScriptableBlocks.Count == 0)
            {
                ScriptableBlocks.AddRange(Resources.LoadAll<Scriptable.BlockObject>(BLOCK_SCRIPTABLE_PATH));
            }

            return ScriptableItems.Find(x => x.id == id);
        }
    }
}