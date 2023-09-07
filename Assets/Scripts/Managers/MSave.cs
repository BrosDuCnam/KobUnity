using System.Collections.Generic;
using Components.UI.Game.Inventory;
using IngameDebugConsole;
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
                DebugLogConsole.AddCommand("get_save", "Get the save", GetSave);
                DebugLogConsole.AddCommand<string>("load_save", "Load the save", LoadSave);
            }
            else
            {
                Debug.LogError($"Multiple {nameof(MSave)} components detected.");
                Destroy(this);
            }
        }

        #endregion
        
        [Header("References")]
        [SerializeField] private BaseInventory inventory;

        #region ISavable
        
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
            
            JSONObject json = new JSONObject();
            json.Add("inventory", inventory.Save());
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
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
            
            inventory.Load(json["inventory"].AsObject);
        }

        #endregion

        #region Commands

        public void GetSave()
        {
            JSONObject json = Save();
            Debug.Log(json.ToString());
        }
        
        public void LoadSave(string json)
        {
            // Sometimes DebugLogConsole remove the first { of the json string
            if (!json.StartsWith("{"))
            {
                json = "{" + json;
            }
            
            Load(JSON.Parse(json).AsObject);
        }

        #endregion
    }
}