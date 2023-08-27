using System;
using UnityEditor;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "New Scriptable Item", menuName = "Scriptable/Scriptable Item")]
    public class ScriptableItem : ScriptableObject
    {
        [SerializeField] private ScriptableItem parent;
        [SerializeField] private string id;
        
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxStack = 99;
        [SerializeField] private ConsumablesData consumablesData;
        
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