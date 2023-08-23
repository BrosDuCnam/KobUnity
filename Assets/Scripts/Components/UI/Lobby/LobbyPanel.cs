using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Components.UI.Lobby
{
	public class LobbyPanel : MonoBehaviour
	{
		[Header("Refs")]
		[SerializeField] private List<CanvasGroup> canvasGroups;
	
		[Header("Settings")]
		[SerializeField] private float fadeDuration = 0.25f;
		[SerializeField] private float displayDelay = 0.1f;
		
		
		private Sequence _sequence;
		
		public Sequence Display(bool display)
		{
			_sequence?.Kill();
			_sequence = DOTween.Sequence();

			if (display) gameObject.SetActive(true);
			else _sequence.OnKill(() => gameObject.SetActive(false));
			
			for (int i = 0; i < canvasGroups.Count; i++)
			{
				CanvasGroup canvasGroup = canvasGroups[i];

				canvasGroup.alpha = display? 0 : 1;
				canvasGroup.interactable = display;
				canvasGroup.blocksRaycasts = display;
				
				_sequence.Insert(displayDelay * i, canvasGroup.DOFade(display ? 1 : 0, fadeDuration).Play());
			}

			return _sequence;
		}
	}
}