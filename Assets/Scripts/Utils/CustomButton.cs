﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
    public class CustomButton : Selectable, ISubmitHandler
    {
        [Header("Settings")]
        [SerializeField] private float cooldown = 0.25f;
        private float _lastPressTime = -Mathf.Infinity;
        private bool CanPress => Time.time - _lastPressTime > cooldown;
        
        [Header("Events")]
        [SerializeField] public UnityEngine.Events.UnityEvent onPressed = new (); // Called when the button is pressed.
        [SerializeField] public UnityEngine.Events.UnityEvent onSelected = new (); 
        [SerializeField] public UnityEngine.Events.UnityEvent onDeselected = new ();

        private bool _skipNextStateChange;
        private bool _isSelected;
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_skipNextStateChange)
            {
                _skipNextStateChange = false;
                return;
            }
            
            base.DoStateTransition(state, instant);

            switch (state)
            {
                case SelectionState.Normal:
                    ApplyDeselected();
                    break;
                case SelectionState.Highlighted: 
                    ApplySelect();
                    break;
                case SelectionState.Pressed: 
                    ApplyPress();
                    break;
                case SelectionState.Selected:
                    ApplySelect();
                    break;
                case SelectionState.Disabled:
                    break;
                default:
                    break;
            }
        }
        
        private void ApplySelect()
        {
            if (_isSelected)
                return;
            
            _isSelected = true;
            onSelected.Invoke();
            OnSelect();
        }
        
        private void ApplyDeselected()
        {
            if (!_isSelected)
                return;
            
            _isSelected = false;
            onDeselected.Invoke();
            OnDeselect();
        }
        
        private void ApplyPress()
        {
            if (!CanPress) return;
            _lastPressTime = Time.time;
            
            onPressed.Invoke();
            OnPressed();
        }

        public virtual void OnSelect() { }
        public virtual void OnDeselect() { }
        public virtual void OnPressed() { }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            _skipNextStateChange = true; // Prevents the button from re-apply last state
            base.OnPointerEnter(eventData);
            
            DoStateTransition(SelectionState.Highlighted, false);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            _skipNextStateChange = true; // Prevents the button from re-apply last state
            base.OnPointerExit(eventData);
            
            DoStateTransition(SelectionState.Normal, false);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;
            
            UISystemProfilerApi.AddMarker("UIBehaviour.OnSubmit", this);
            DoStateTransition(SelectionState.Pressed, false);
        }
        
    }
}