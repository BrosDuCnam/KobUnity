
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyButton : UIBehaviour<LobbyButton.LobbyButtonData>, IDisplayable
    {
        public struct LobbyButtonData
        {
            public string text;
            public Action onPressed;
        }
        
        [Header("UI References")]
        [SerializeField] private TMPro.TextMeshProUGUI _text;
        [SerializeField] private Image _symbol;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private Sequence _symbolSequence;
        private UnityAction _onPressed;
        
        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;

        private new void Awake()
        {
            base.Awake();
            
            _symbol.gameObject.SetActive(false);
            
            onPressed.AddListener(OnPressed);
            
            onSelected.AddListener(OnHoverEnter);
            onDeselected.AddListener(OnHoverExit);
        }
        
        public override void Refresh(LobbyButtonData data)
        {
            if (_onPressed != null) onPressed.RemoveListener(_onPressed);
            
            _onPressed = () => data.onPressed?.Invoke();
            onPressed.AddListener(_onPressed);
            
            _text.text = data.text;
        }

        public  void OnPressed()
        {
            _symbolSequence?.Kill();
            _symbolSequence = DOTween.Sequence();
            
            _symbolSequence.Append(_symbol.rectTransform.DOAnchorPosX(10, 0.1f));
            _symbolSequence.Append(_symbol.rectTransform.DOAnchorPosX(0, 0.1f));
            
            _symbolSequence.Play();
        }

        public void OnHoverEnter()
        {
            _symbolSequence?.Kill();
            _symbolSequence = DOTween.Sequence();
            
            _symbol.gameObject.SetActive(true);
            Color color = _symbol.color;
            color.a = 0f;
            _symbol.color = color;
            
            _symbolSequence.Append(_symbol.DOFade(1f, 0.3f));

            _symbolSequence.Play();
        }
        
        public void OnHoverExit()
        {
            _symbolSequence?.Kill();
            _symbolSequence = DOTween.Sequence();
            
            _symbolSequence.Append(_symbol.DOFade(0f, 0.3f));
            _symbolSequence.AppendCallback(() => _symbol.gameObject.SetActive(false));

            _symbolSequence.Play();
        }
    }
}