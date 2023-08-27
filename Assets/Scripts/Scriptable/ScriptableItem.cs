using System;
using UnityEditor;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "New Scriptable Item", menuName = "Scriptable/Scriptable Item")]
    public class ScriptableItem : ScriptableObject
    {
        [SerializeField] public ScriptableItem parent;
        [SerializeField] public string id;
        
        [SerializeField] public string itemName;
        [SerializeField] public Sprite icon;
        [SerializeField] public int maxStack = 99;
        [SerializeField] public ConsumablesData consumablesData;
        
        [Serializable]
        public struct ConsumablesData
        {
            public int health;
            public int thirst;
            public int hunger;
        }
        
        public void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }
    }
}