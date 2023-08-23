using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Components.UI.Lobby
{
    public class JoinButton : CustomButton, UIBehaviour<JoinButton.JoinButtonData>, IDisplayable
    {
        public struct JoinButtonData
        {
            public string text;
        }
        
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _dotsCanvasGroup;
        [SerializeField] private List<Image> _dots;
        [SerializeField] private Image _background;

        [Header("Settings")] 
        [SerializeField] private float _normalBgAlpha = 0f;
        [SerializeField] private float _selectedBgAlpha = 0.2f;
        [SerializeField] private float _cooldown = 0.5f;
        [SerializeField] private UnityEvent<string> _onPressed = new ();

        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;
        
        private Sequence _bgSequence;
        private Sequence _dotsSequence;
        private bool _isLoading;

        // Cooldown
        private bool CanPress => Time.time - _lastPressTime > _cooldown;
        private float _lastPressTime;
        
        protected new void Start()
        {
            SetupDotsSequence();
            
            MNetwork.Singleton.onAction.AddListener((action) =>
            {
                switch (action)
                {
                    case MNetwork.NetworkAction.TryJoinLobby:
                        SetLoading(true);
                        break;
                    case MNetwork.NetworkAction.FinishJoinLobby:
                        if (MNetwork.Singleton.Lobby != null)
                            LobbyUI.Singleton.LoadPanel(LobbyUI.Panel.Room);

                        SetLoading(false);
                        break;
                }
            });
        }
        
        public void SetLoading(bool loading)
        {
            _isLoading = loading;
            
            _text.DOFade(loading ? 0f : 1f, 0.3f);
            
            _dotsCanvasGroup.DOFade(loading ? 1f : 0f, 0.3f);
            
            if (loading)
                _dotsSequence.Restart();
            else
                _dotsSequence.Pause();
        }
        
        public void Refresh(JoinButtonData data)
        {
            _text.text = data.text;
        }

        public override void OnSelect()
        {
            if (_isLoading) return;
            
            _bgSequence?.Kill();
            _bgSequence = DOTween.Sequence();
            
            _bgSequence.Append(_background.DOFade(_selectedBgAlpha, 0.3f));
            
            _bgSequence.Play();
        }
        
        public override void OnDeselect()
        {
            _bgSequence?.Kill();
            _bgSequence = DOTween.Sequence();
            
            _bgSequence.Append(_background.DOFade(_normalBgAlpha, 0.3f));
            
            _bgSequence.Play();
        }
        
        public override void OnPressed()
        {
            if (_isLoading) return;
            if (!CanPress) return;
            
            _lastPressTime = Time.time;
            
            _bgSequence?.Kill();
            _onPressed?.Invoke(LobbyUI.Singleton.lobbyInputField.Text);
        }

        private void SetupDotsSequence()
        {
            _dotsSequence = DOTween.Sequence();

            // Appearing
            for (int i = 0; i < _dots.Count; i++)
            {
                Image dot = _dots[i];
                
                _dotsSequence.Insert(0.1f * i, 
                    dot.DOFade(1f, 0.3f).SetEase(Ease.OutCubic));
            }
            
            float duration = 0.1f * _dots.Count + 0.2f;
            
            // Disappearing
            for (int i = 0; i < _dots.Count; i++)
            {
                Image dot = _dots[i];
                
                _dotsSequence.Insert(duration + 0.1f * i, 
                    dot.DOFade(0f, 0.3f).SetEase(Ease.OutCubic));
            }
            
            _dotsSequence.SetLoops(-1);
        }
    }
}