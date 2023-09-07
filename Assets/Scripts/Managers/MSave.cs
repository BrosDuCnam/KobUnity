using System.Collections.Generic;
using Interfaces;
using SimpleJSON;
using UnityEngine;

namespace Managers
{
    public class MSave : MonoBehaviour, ISavable
    {

        #region Singleton
        
        public static MSave Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError($"Multiple {nameof(MSave)} components detected.");
                Destroy(this);
            }
        }

        #endregion
        
        public List<ISavable> Children { get; set; } = new List<ISavable>();
        
        public JSONObject Save()
        {
            /* Example
             * foreach (var child in Children)
             * {
             *     child.Save();
             * }
             */
            
            /* TODO: to save when implemented :
             * - Players
             * - Islands
             * - Raft
             * - Grounds Items ? ( if not saved in there own parent like islands or raft )
             */
            
            return new JSONObject();
        }

        public void Load(JSONObject json)
        {
            /* Example
             * foreach (var child in Children)
             * {
             *     child.Load();
             * }
             */
        }
    }
}