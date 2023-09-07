using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Components.UI.Lobby
{
    public class SaveSection : MonoBehaviour, UIBehaviour<SaveSection.SaveSectionData>
    {
        public struct SaveSectionData
        {
            public string saveName;
            public int? timePlayed; // In milliseconds
            public DateTime? lastPlayed;
        }

        [Header("References")]
        [SerializeField] private TMP_InputField _saveNameInputField;

        [SerializeField] private TextMeshProUGUI _timePlayed;
        [SerializeField] private TextMeshProUGUI _lastPlayed;
        
        protected void OnEnable()
        {
            _saveNameInputField.enabled = MNetworkHandler.Instance.lobbyHandler.IsLobbyOwner;
        }
        
        public void Refresh(SaveSectionData newItem)
        {
            _saveNameInputField.text = newItem.saveName;

            if (newItem.timePlayed == null)
            {
                _timePlayed.text = "0m 0s";
                _lastPlayed.text = "Never";
                return;
            }

            string timePlayed = $"{newItem.timePlayed / 1000 / 60}m {newItem.timePlayed / 1000 % 60}s";
            _timePlayed.text = timePlayed;

            if (newItem.lastPlayed == null) // Should never happen
            {
                _lastPlayed.text = "Never";
            }
            else
            {
                _lastPlayed.text = newItem.lastPlayed.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}