using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
    public abstract class UIBehaviour<T> : Selectable, ISubmitHandler where T : struct
    {
        [Header("Events")]
        [SerializeField] public UnityEngine.Events.UnityEvent onPressed = new (); // Called when the button is pressed.
        [SerializeField] public UnityEngine.Events.UnityEvent onSelected = new (); 
        [SerializeField] public UnityEngine.Events.UnityEvent onDeselected = new ();

        private bool _isSelected;
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            switch (state)
            {
                case SelectionState.Normal:
                    OnDeselected();
                    break;
                case SelectionState.Highlighted: 
                    OnSelected();
                    break;
                case SelectionState.Pressed: 
                    OnPressed();
                    break;
                case SelectionState.Selected:
                    OnSelected();
                    break;
                case SelectionState.Disabled:
                    break;
                default:
                    break;
            }
        }
        
        protected void OnSelected()
        {
            if (_isSelected)
                return;
            
            _isSelected = true;
            onSelected.Invoke();
        }
        
        protected void OnDeselected()
        {
            if (!_isSelected)
                return;
            
            _isSelected = false;
            onDeselected.Invoke();
        }
        
        protected void OnPressed()
        {
            onPressed.Invoke();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            onSelected.Invoke();
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            
            onDeselected.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;
            
            UISystemProfilerApi.AddMarker("UIBehaviour.OnSubmit", this);
            DoStateTransition(SelectionState.Pressed, false);
        }

        public abstract void Refresh(T data);
        
    }
}