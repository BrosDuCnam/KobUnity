using System.Collections.Generic;
using Components.UI.Game.Inventory;
using IngameDebugConsole;
using Interfaces;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Serialization;
using NetworkPlayer = Network.NetworkPlayer;

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
                DebugLogConsole.AddCommand<int>("get_random_inventory", "Get a random inventory", GetRandomInventory);
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
        [SerializeField] public List<NetworkPlayer> players; // fill by NetworkPlayer itself
        
        private JSONObject _save;

        #region ISavable
        
        public JSONObject Save()
        {
            /* TODO: to save when implemented :
             * - Players
             * - Islands
             * - Raft
             * - Grounds Items ? ( if not saved in there own parent like islands or raft )
             */
            
            JSONObject json = new JSONObject();
            
            JSONArray players = new JSONArray();
            foreach (var player in this.players)
            {
                if (player == null) continue;
                players.Add(player.Save());
            }
            
            json.Add("players", players);
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("players", new JSONArray());
            
            return json;
        }

        public void Load(JSONObject json)
        {
            Load(json, false);
        }

        public void Load(JSONObject json, bool dirty)
        {
            if (!dirty) _save = json;
            
            foreach (var player in players)
            {
                LoadPlayer(player.UUID, json);
            }
        }
        
        public void LoadPlayer(string id, JSONObject json = null)
        {
            JSONObject tempSave = json ?? _save ?? GetDefaultSave();
            
            NetworkPlayer player = players.Find(p => p.UUID == id);
            if (player == null)
            {
                Debug.LogError($"Player with id {id} not found.");
                return;
            }
            
            foreach (var jsonPlayer in tempSave["players"].AsArray)
            {
                if (jsonPlayer.Value["id"].Value == id)
                {
                    player.Load(jsonPlayer.Value.AsObject);
                    return;
                }
            }
        }

        #endregion

        #region Commands

        public void GetSave()
        {
            JSONObject json = Save();
            Debug.Log(json.ToString());
            GUIUtility.systemCopyBuffer = json.ToString(4);
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

        public void GetRandomInventory(int size)
        {
            JSONObject json = new JSONObject();
            JSONArray items = new JSONArray();
            
            for (int i = 0; i < size; i++)
            {
                JSONObject item = new JSONObject();
                item.Add("id", Random.Range(0, 2) == 0 ? 0 : 7385);
                item.Add("amount", Random.Range(1, 100));
                items.Add(item);
            }
            
            json.Add("items", items);
            
            Debug.Log(json.ToString());
            GUIUtility.systemCopyBuffer = json.ToString(4);
        }
        
        #endregion
    }
}