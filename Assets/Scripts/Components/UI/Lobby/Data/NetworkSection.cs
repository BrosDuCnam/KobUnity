using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class NetworkSection : MonoBehaviour, UIBehaviour<NetworkSection.NetworkSectionData>
    {
        public struct NetworkSectionData
        {
            public string lobbyCode;
            public List<string> players;
        }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _lobbyCode;
        [SerializeField] private TextMeshProUGUI _players;

        private void Start()
        {
            MNetwork.Singleton.lobbyUpdated.AddListener((lobby) =>
            {
                Refresh(new NetworkSectionData
                {
                    lobbyCode = lobby.Data["code"].Value,
                    players = lobby.Players.Select(p => p.Data["name"].Value).ToList(),
                });
            });
        }

        public void Refresh(NetworkSectionData data)
        {
            _lobbyCode.text = data.lobbyCode;
            _players.text = string.Join(", ", data.players);
        }
    }
}