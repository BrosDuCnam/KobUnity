using System;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    public class TextTint : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        [Header("Tint Colors")]
        [SerializeField] private ColorBlock _colors;
        
        private TextMeshProUGUI _text;
        private Sequence _sequence;
        
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
            MLobby.Instance.onHoverButton.AddListener((obj) =>
            {
                if (obj == _button.gameObject)
                {
                    OnHover();
                }
                else
                {
                    OnExit();
                }
            });
        }

        private void OnClick()
        {
            _sequence?.Kill();
            
            _sequence = DOTween.Sequence()
                .Append(_text.DOColor(_colors.pressedColor, _colors.fadeDuration))
                .Append(_text.DOColor(_colors.selectedColor, _colors.fadeDuration));
            
            _sequence.Play();
        }
        
        private void OnHover()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_text.DOColor(_colors.selectedColor, _colors.fadeDuration));
            
            _sequence.Play();
        }
        
        private void OnExit()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_text.DOColor(_colors.normalColor, _colors.fadeDuration));
            
            _sequence.Play();
        }
        
        private void OnDestroy()
        {
            _sequence.Kill();
            
            _button.onClick.RemoveListener(OnClick);
            MLobby.Instance.onHoverButton.RemoveListener((obj) =>
            {
                if (obj == _button.gameObject)
                {
                    OnHover();
                }
                else
                {
                    OnExit();
                }
            });
        }
    }
}