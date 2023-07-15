using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI.Lobby
{
    public class LeftButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float hoverOffset = 10f;
        
        [Header("References")]
        [SerializeField] private RectTransform body;

        private Button _button;
        private Sequence _hoverSequence;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _hoverSequence = SetupHoverSequence();
        }
        
        public void SetHover(bool state)
        {
            if (state)
            {
                _hoverSequence.Restart();
            }
            else
            {
                _hoverSequence.Pause();
                body.DOLocalMoveX(0, 0.1f);
            }
        }
        
        private Sequence SetupHoverSequence()
        {
            return DOTween.Sequence()
                .Append(body.DOLocalMoveX(hoverOffset, 0.5f).SetEase(Ease.Linear))
                .Append(body.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear))
                .SetLoops(-1);
        }
    }
}