using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public GameObject lobbyButtonPrefab;
        [SerializeField] public Transform lobbyButtonParent;

        [Header("Settings")]
        [SerializeField] private float _buttonDisplayDelay = 0.1f;
        
        private readonly UIPooling<LobbyButton, LobbyButton.LobbyButtonData> _lobbyButtonPool = new();

        public enum Panel
        {
            Main,
            Play,
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
            }
        }

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
                    onPressed = () => Debug.Log("New Game"),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Load Game",
                    onPressed = () => Debug.Log("Load Game"),
                },
                new LobbyButton.LobbyButtonData()
                {
                    text = "Back",
                    onPressed = () => LoadPanel(Panel.Main),
                },
            };
            
            StartCoroutine(LoadPanelCoroutine(data));
        }
        
        private IEnumerator LoadPanelCoroutine(List<LobbyButton.LobbyButtonData> data)
        {
            yield return DisplayButtons(false).Play().WaitForCompletion();
            

            _lobbyButtonPool.Refresh(data, lobbyButtonPrefab, lobbyButtonParent, (button) =>
            {
                button.Display(false, true);
            });
            
            yield return DisplayButtons(true).Play().WaitForCompletion();
        }
        
        public Sequence DisplayButtons(bool display)
        {
            Sequence sequence = DOTween.Sequence();

            if (_lobbyButtonPool.items == null) return sequence;
            
            int j = 0;
            foreach (LobbyButton button in _lobbyButtonPool.items)
            {
                if (!button.gameObject.activeSelf) continue;
                
                sequence.Insert(j * _buttonDisplayDelay, button.Display(display));
                j++;
            }
            
            return sequence;
        }
    }
}