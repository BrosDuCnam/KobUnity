using System;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyInputField : MonoBehaviour, IDisplayable
    {
        [Header("Settings")]
        [SerializeField] private float _borderWidth = 2;
        
        [Header("References")]
        [SerializeField] private TMPro.TMP_InputField _inputField;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Borders")]
        [SerializeField] private RectTransform _borderTopLeft;
        [SerializeField] private RectTransform _borderTopRight;
        [SerializeField] private RectTransform _borderLeft;
        [SerializeField] private RectTransform _borderRight;
        
        private Sequence _borderSequence;
        
        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;

        private void Awake()
        {
            _inputField.onSelect.AddListener(OnSelect);
            _inputField.onDeselect.AddListener(OnDeselect);
        }

        public void OnSelect(string text)
        {
            _borderSequence?.Kill();
            _borderSequence = GetSelectionSequence();
            
            _borderSequence.Play();
        }
        
        public void OnDeselect(string text)
        {
            _borderSequence?.Kill();
            _borderSequence = GetDeselectionSequence();
            
            _borderSequence.Play();
        }
        
        public Tween Display()
        {
            return _canvasGroup.DOFade(1, 0.15f);
        }
        
        public Sequence GetSelectionSequence()
        {
            RectTransform rectTransform = _borderRight.parent.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(_borderLeft.DOSizeDelta(new Vector2(_borderWidth, rectTransform.rect.height), 0.15f));
            sequence.Join(_borderRight.DOSizeDelta(new Vector2(_borderWidth, rectTransform.rect.height), 0.15f));
            
            sequence.Append(_borderTopLeft.DOSizeDelta(new Vector2(rectTransform.rect.width/2, _borderWidth), 0.15f));
			sequence.Join(_borderTopRight.DOSizeDelta(new Vector2(rectTransform.rect.width/2, _borderWidth), 0.15f));

            return sequence;
        }

        public Sequence GetDeselectionSequence()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_borderTopLeft.DOSizeDelta(new Vector2(0, _borderWidth), 0.15f));
            sequence.Join(_borderTopRight.DOSizeDelta(new Vector2(0, _borderWidth), 0.15f));

            sequence.Append(_borderLeft.DOSizeDelta(new Vector2(_borderWidth, 0), 0.15f));
            sequence.Join(_borderRight.DOSizeDelta(new Vector2(_borderWidth, 0), 0.15f));

            return sequence;
        }
    }
}