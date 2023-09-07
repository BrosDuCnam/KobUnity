using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        #region Singleton

        public static LobbyUI Singleton { get; private set; }
        
        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        #endregion
        
        [Header("References")]
        [SerializeField] public LobbyInputField lobbyInputField;

        [Header("Settings")]
        [SerializeField] private SerializedDictionary<Panel, LobbyPanel> panels;
        [SerializeField] private float pageDisplayDelay = 0.1f;
        
        public Panel CurrentPanel => panels.FirstOrDefault(p => p.Value == _currentPanel).Key;
        
        private Sequence _sequence;
        private LobbyPanel _currentPanel;
        private LobbyPanel _incomingPanel;
        
        public enum Panel
        {
            Main,
            Play,
            Join,
            Room,
        }

        private void Start()
        {
            LoadPanel(Panel.Main);
            
            MNetworkHandler.Instance.lobbyHandler.onLobbyChanged.AddListener((lobby) =>
            {
                if (lobby == null && CurrentPanel == Panel.Room)
                {
                    LoadPanel(Panel.Main);
                }
                else if (lobby != null && CurrentPanel != Panel.Room)
                {
                    LoadPanel(Panel.Room);
                }
            });
        }

        public void LoadPanel(string panel)
        {
            if (Enum.TryParse(panel, true, out Panel panelEnum))
            {
                LoadPanel(panelEnum);
                return;
            }
            
            LoadPanel(panelEnum);
        }
        
        public void LoadPanel(Panel panel)
        {
            if (!panels.ContainsKey(panel))
            {
                Debug.LogError($"Panel {panel} not found!");
                return;
            }
            
            // Prevent loading the same panel twice
            if (_currentPanel == panels[panel] ||
                _incomingPanel == panels[panel]) return;
            
            _incomingPanel = panels[panel];
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            if (_currentPanel != null)
            {
                _sequence.Append(_currentPanel.Display(false));
                _sequence.AppendInterval(pageDisplayDelay);
            }
            _sequence.Append(panels[panel].Display(true));
            _sequence.OnKill(() =>
            {
                _currentPanel = panels[panel];
                _incomingPanel = null;
            });
            
            _sequence.Play();
        }
        
        protected void OnDestroy()
        {
            _sequence?.Kill();
        }
        
        public void CreateRoom()
        {
            MNetworkHandler.Instance.lobbyHandler.CreateLobby();
        }
        
        public void JoinRoom(string lobbyId)
        {
            MNetworkHandler.Instance.lobbyHandler.JoinLobbyByCode(lobbyId);
        }

        public void StartGame()
        {
            if (MNetworkHandler.Instance.lobbyHandler.Lobby == null) return;
            if (!MNetworkHandler.Instance.lobbyHandler.IsLobbyOwner) return;
            
            MNetworkHandler.Instance.lobbyHandler.StartGame();
        }
    }
}