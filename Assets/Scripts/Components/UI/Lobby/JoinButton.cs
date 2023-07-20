using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class JoinButton : UIBehaviour<JoinButton.JoinButtonData>, IDisplayable
    {
        public struct JoinButtonData
        {
            public string text;
        }
        
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _text;
        
        [Header("Settings")]
        [SerializeField] private float _normalSpacing = 10;
        [SerializeField] private float _selectedSpacing = 15;
        [SerializeField] private float _selectedPressSpacing = 5;

        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;
        
        private Sequence _textSequence;
        
        public override void Refresh(JoinButtonData data)
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
        }
    }
}