using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
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
        [SerializeField] private float _buttonDisplayDelay = 0.1f;
        
        private readonly UIPooling<LobbyButton, LobbyButton.LobbyButtonData> _lobbyButtonPool = new();

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

        public void LoadPanel(Panel panel)
        {
            switch (panel)
            {
                case Panel.Main:
                    LoadMain();
                    break;
                case Panel.Play:
                    LoadPlay();
                    break;
                case Panel.Join:
                    LoadJoin();
                    break;
                case Panel.Room:
                    LoadRoom();
                    break;
            }
        }

        #region LoadPanel

        private void LoadMain()
        {
            List<LobbyButton.LobbyButtonData> data = new List<LobbyButton.LobbyButtonData>()
            {
                new LobbyButton.LobbyButtonData()
                {
                    text = "Play",
                    onPressed = () => LoadPanel(Panel.Play),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Settings",
                    onPressed = () => Debug.Log("Settings"),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Quit",
                    onPressed = () => Debug.Log("Quit"),
                },
            };
            
            StartCoroutine(LoadPanelCoroutine(data));
        }
        
        private void LoadPlay()
        {
            List<LobbyButton.LobbyButtonData> data = new List<LobbyButton.LobbyButtonData>()
            {
                new LobbyButton.LobbyButtonData()
                {
                    text = "New Game",
                    onPressed = () => MNetwork.Singleton.CreateLobby(),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Load Game",
                    onPressed = () => LoadPanel(Panel.Join),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Back",
                    onPressed = () => LoadPanel(Panel.Main),
                },
            };
            
            StartCoroutine(LoadPanelCoroutine(data));
        }
        
        private void LoadJoin()
        {
            List<LobbyButton.LobbyButtonData> data = new List<LobbyButton.LobbyButtonData>()
            {
                new LobbyButton.LobbyButtonData()
                {
                    text = "Back",
                    onPressed = () => LoadPanel(Panel.Main),
                }
            };
            
            StartCoroutine(LoadPanelCoroutine(data, () =>
            {
                lobbyInputField.gameObject.SetActive(true);
                joinButton.gameObject.SetActive(true);
            }));
        }

        private void LoadRoom()
        {
            List<LobbyButton.LobbyButtonData> data = new List<LobbyButton.LobbyButtonData>();
            
            LobbyData.RoomSettingsData roomSettingsData = new LobbyData.RoomSettingsData()
            {
                lastPlayed = DateTime.Now,
                lobbyName = "Default Lobby",
                lobbyCode = "123456",
                timePlayed = 123456,
            };
            
            StartCoroutine(LoadPanelCoroutine(data, () =>
            {
                lobbyData.Refresh(roomSettingsData);
                lobbyData.gameObject.SetActive(true);
            }));
        }
        
        #endregion
        
        
        private IEnumerator LoadPanelCoroutine(List<LobbyButton.LobbyButtonData> data, Action afterHide = null)
        {
            yield return DisplayObjects(false, true).Play().WaitForCompletion();
            
            afterHide?.Invoke();

            var refreshResult = _lobbyButtonPool.Refresh(data, lobbyButtonPrefab, lobbyButtonParent);
            
            refreshResult.activeItems.ForEach(button =>
            {
                ((IDisplayable)button).Display(false, false, true);
                button.gameObject.SetActive(true);
            });
            
            yield return DisplayObjects(true).Play().WaitForCompletion();
        }
        
        public Sequence DisplayObjects(bool display, bool changeActive = false)
        {
            Sequence sequence = DOTween.Sequence();

            if (_lobbyButtonPool.items == null) return sequence;
            
            int j = 0;
            foreach (IDisplayable obj in lobbyButtonParent.GetComponentsInChildren<MonoBehaviour>().OfType<IDisplayable>().ToArray())
            {
                sequence.Insert(j * _buttonDisplayDelay, obj.Display(display, changeActive));
                j++;
            }
            
            return sequence;
        }
    }
}