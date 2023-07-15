using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Components.UI.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Selector selector;
        [SerializeField] private float selectorOffset = 10f;
        
        private List<LeftButton> _buttons;
        private GameObject _lastSelected;
        
        private void Awake()
        {
            _buttons = GetComponentsInChildren<LeftButton>(true).ToList();
            
            for (int i = _buttons.Count - 1; i >= 0; i--)
            {
                var button = _buttons[i];
                
                if (!button.TryGetComponent(out EventTrigger eventTrigger))
                {
                    _buttons.RemoveAt(i);
                    continue;
                }
                
                eventTrigger.triggers.Add(new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter,
                    callback = new EventTrigger.TriggerEvent()
                });
                
                eventTrigger.triggers[^1].callback.AddListener((data) => OnHoverButton(button));
            }
        }
        
        private void Start()
        {
            MLobby.Instance.onHoverButton.AddListener(OnSelectionChanged);
        }

        private void OnSelectionChanged(GameObject selected)
        {
            if (selected != null && selected.TryGetComponent(out LeftButton button))
            {
                OnHoverButton(button);

                // Set hover to false for all other buttons.
                foreach (var otherButton in _buttons)
                {
                    if (otherButton == button) continue;
                    otherButton.SetHover(false);
                }
            }
            else
            {
                selector.SetActive(false);

                // Set hover to false for all buttons.
                foreach (var otherButton in _buttons)
                {
                    otherButton.SetHover(false);
                }
            }
        }

        private void OnHoverButton(LeftButton leftButton)
        {
            if (!_buttons.Contains(leftButton)) return;
            
            // Make sure this button as hover for the event system.
            EventSystem.current.SetSelectedGameObject(leftButton.gameObject);
            
            // Move the selector to the left of the button.
            RectTransform buttonRect = (RectTransform) leftButton.transform;
            
            selector.gameObject.SetActive(true);
            var targetPos = buttonRect.position - new Vector3(buttonRect.rect.width / 2f + selectorOffset, 0f, 0f);
            
            selector.SetActive(true);
            selector.rectTransform.DOMove(targetPos, 0.1f);
            
            leftButton.SetHover(true);
        }
    }
}