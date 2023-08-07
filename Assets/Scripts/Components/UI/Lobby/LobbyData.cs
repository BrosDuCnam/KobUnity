using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyData : MonoBehaviour, UIBehaviour<LobbyData.RoomSettingsData>
    {
        public struct RoomSettingsData
        {
            public string lobbyName;
            public int? timePlayed; // In milliseconds
            public DateTime? lastPlayed;
            public string lobbyCode;
        }
        
        [Header("References")]
        [SerializeField] private TMP_InputField _lobbyNameInputField;

        [SerializeField] private TextMeshProUGUI _timePlayed;
        [SerializeField] private TextMeshProUGUI _lastPlayed;
        
        [SerializeField] private TextMeshProUGUI _lobbyCode;

        protected void OnEnable()
        {
            _lobbyNameInputField.enabled = MNetwork.Singleton.IsHost;
        }
        
        public void Refresh(RoomSettingsData data)
        {
            _lobbyNameInputField.text = data.lobbyName;
            _lobbyCode.text = data.lobbyCode;

            if (data.timePlayed == null)
            {
                _timePlayed.text = "0m 0s";
                _lastPlayed.text = "Never";
                return;
            }

            string timePlayed = $"{data.timePlayed / 1000 / 60}m {data.timePlayed / 1000 % 60}s";
            _timePlayed.text = timePlayed;

            if (data.lastPlayed == null) // Should never happen
            {
                _lastPlayed.text = "Never";
            }
            else
            {
                _lastPlayed.text = data.lastPlayed.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}