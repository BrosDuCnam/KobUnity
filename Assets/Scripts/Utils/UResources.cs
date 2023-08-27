using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class UResources
    {
        // Resources paths
        public const string ITEM_SCRIPTABLE_PATH = "Contents/Items/";
        
        // Cached resources
        private static readonly List<Scriptable.ScriptableItem> ScriptableItems = new();
        
        // Getters
        public static Scriptable.ScriptableItem GetScriptableItemById(string id)
        {
            if (ScriptableItems.Count == 0)
            {
                ScriptableItems.AddRange(Resources.LoadAll<Scriptable.ScriptableItem>(ITEM_SCRIPTABLE_PATH));
            }

            return ScriptableItems.Find(x => x.id == id);
        }
    }
}