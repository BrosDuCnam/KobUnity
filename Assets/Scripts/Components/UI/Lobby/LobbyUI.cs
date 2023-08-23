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
        [SerializeField] public GameObject lobbyButtonPrefab;
        [SerializeField] public Transform lobbyButtonParent;
        [SerializeField] public LobbyInputField lobbyInputField;
        [SerializeField] public LobbyData lobbyData;
        [SerializeField] public JoinButton joinButton;

        [Header("Settings")]
        [SerializeField] private SerializedDictionary<Panel, LobbyPanel> _panels;
        [SerializeField] private float _pageDisplayDelay = 0.1f;
        
        private Sequence _sequence;
        private LobbyPanel _currentPanel;
        
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
            if (!_panels.ContainsKey(panel))
            {
                Debug.LogError($"Panel {panel} not found!");
                return;
            }
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            if (_currentPanel != null)
            {
                Debug.Log("[DEBUG/LobbyUI]: Hide last panel");
                
                _sequence.Append(_currentPanel.Display(false));
                _sequence.AppendInterval(_pageDisplayDelay);
            }
            _sequence.Append(_panels[panel].Display(true));
            _sequence.AppendCallback(() => _currentPanel = _panels[panel]);
            
            _sequence.Play();
        }
        
        public async void CreateRoom()
        {
            await MNetwork.Singleton.CreateLobby();
            if (MNetwork.Singleton.Lobby == null) return;
            
            LoadPanel(Panel.Room);
        }
    }
}