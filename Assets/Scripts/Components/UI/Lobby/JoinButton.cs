using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        
        [Header("Settings")]
        [SerializeField] private float _normalSpacing = 10;
        [SerializeField] private float _selectedSpacing = 15;
        [SerializeField] private float _selectedPressSpacing = 5;

        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;
        
        private Sequence _textSequence;
        private Sequence _dotsSequence;

        protected void Start()
        {
            SetupDotsSequence();
            
            MNetwork.Singleton.onAction.AddListener((action) =>
            {
                switch (action)
                {
                    case MNetwork.ActionEnum.TryJoinLobby:
                        SetLoading(true);
                        break;
                    case MNetwork.ActionEnum.FinishJoinLobby:
                        SetLoading(false);
                        break;
                }
            });
        }
        
        public void SetLoading(bool loading)
        {
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
            _textSequence?.Kill();
            _textSequence = DOTween.Sequence();
            
            _textSequence.Append(
                DOTween.To(() => _text.characterSpacing, x => _text.characterSpacing = x, _selectedSpacing, 0.1f));
            
            _textSequence.Play();
        }
        
        public override void OnDeselect()
        {
            _textSequence?.Kill();
            _textSequence = DOTween.Sequence();
            
            _textSequence.Append(
                DOTween.To(() => _text.characterSpacing, x => _text.characterSpacing = x, _normalSpacing, 0.1f));
            
            _textSequence.Play();
        }
        
        public override void OnPressed()
        {
            _textSequence?.Kill();
            _textSequence = DOTween.Sequence();
            
            _textSequence.Append(
                DOTween.To(() => _text.characterSpacing, x => _text.characterSpacing = x, _selectedPressSpacing, 0.1f));
            _textSequence.Append(
                DOTween.To(() => _text.characterSpacing, x => _text.characterSpacing = x, _selectedSpacing, 0.1f));
            
            _textSequence.Play();
            
            MNetwork.Singleton.JoinLobbyByCode(LobbyUI.Singleton.lobbyInputField.Text);
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