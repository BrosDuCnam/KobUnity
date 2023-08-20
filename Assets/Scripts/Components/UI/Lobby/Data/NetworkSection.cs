using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class NetworkSection : MonoBehaviour, UIBehaviour<NetworkSection.NetworkSectionData>
    {
        public struct NetworkSectionData
        {
            public bool online;
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
                if (lobby == null)
                {
                    Refresh(new NetworkSectionData
                    {
                        online = false,
                    });
                    return;
                }
                
                Refresh(new NetworkSectionData
                {
                    online = true,
                    lobbyCode = lobby.Data["code"].Value,
                    players = lobby.Players.Select(p => p.Data["name"].Value).ToList(),
                });
            });
        }

        public void Refresh(NetworkSectionData data)
        {
            if (!data.online)
            {
                _lobbyCode.text = "..."; // TODO: polish, animate dots
                _players.text = "...";
                return;
            }
            
            _lobbyCode.text = data.lobbyCode;
            _players.text = string.Join(", ", data.players);
        }
    }
}