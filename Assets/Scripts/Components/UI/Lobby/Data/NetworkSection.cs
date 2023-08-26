using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;
using Utils.Network;

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
        [SerializeField] private TextMeshProUGUI _clipboardMessage;
        
        private Sequence _clipboardSequence;
        
        private void Start()
        {
            MNetworkHandler.Instance.lobbyHandler.onLobbyChanged.AddListener((lobby) =>
            {
                NetworkSectionData data = new NetworkSectionData
                {
                    online = lobby != null,
                };
                
                if (lobby != null)
                {
                    data.lobbyCode = lobby.Data[LobbyHandler.KEY_CODE].Value;

                    if (lobby.Players != null)
                    {
                        data.players = lobby.Players
                            .Where(p => p is { Data: not null } && p.Data.ContainsKey("name"))
                            .Select(p => p.Data["name"].Value).ToList();
                    }
                }
                
                Refresh(data);
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
        
        public void PlayClipboardMessage()
        {
            _clipboardSequence?.Kill();
            _clipboardSequence = DOTween.Sequence();
            
            _clipboardSequence.Append(_clipboardMessage.DOFade(1, 0));
            _clipboardSequence.Append(_clipboardMessage.DOFade(0, 0.5f).SetEase(Ease.OutCubic));
            
            _clipboardSequence.Play();
        }
        
        public void CopyLobbyCode()
        {
            GUIUtility.systemCopyBuffer = _lobbyCode.text;
            PlayClipboardMessage();
        }

        protected void OnDestroy()
        {
            _clipboardSequence?.Kill();
        }
    }
}