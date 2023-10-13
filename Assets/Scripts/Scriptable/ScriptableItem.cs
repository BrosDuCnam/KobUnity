using System;
using UnityEditor;
using UnityEngine;

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
            public int[] ids;
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
        }
    }
}