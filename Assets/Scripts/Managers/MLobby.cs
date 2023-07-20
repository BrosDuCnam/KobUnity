using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Components.UI.Lobby;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Managers
{
    public class MLobby : MonoBehaviour
    {
        public static MLobby Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        [Header("Settings")]
        [SerializeField] private float lobbyRefreshInterval = 5f;
        
		[Header("Prefabs References")]
        [SerializeField] public GameObject lobbyItemPrefab;

        [HideInInspector] public GameObject currentSelection;
        [HideInInspector] public UnityEvent<GameObject> onHoverButton = new UnityEvent<GameObject>();
        [HideInInspector] public List<Lobby> lobbies = new List<Lobby>();
        
        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != currentSelection)
            {
                currentSelection = selected;
                onHoverButton.Invoke(selected);
            }
        }
    }
}