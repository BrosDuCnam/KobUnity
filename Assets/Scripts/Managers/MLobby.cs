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
        
        private void Start()
        {
            StartCoroutine(RefreshLobbyListCoroutine());
        }
        
        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != currentSelection)
            {
                currentSelection = selected;
                onHoverButton.Invoke(selected);
            }
        }
        
        private IEnumerator RefreshLobbyListCoroutine()
        {
            while (true)
            {
                if (AuthenticationService.Instance == null || AuthenticationService.Instance.IsSignedIn == false)
                {
                    yield return new WaitForSeconds(lobbyRefreshInterval);
                    continue;
                }
                
                Task<List<Lobby>> task = MNetwork.Singleton.GetLobbies();
                
                yield return new WaitUntil(() => task.IsCompleted);
                lobbies = task.Result;
                
                // LobbyList.LobbyListData lobbyListData = new LobbyList.LobbyListData
                // {
                //     lobbies = task.Result == null ? 
                //         new List<LobbyItem.LobbyItemData>() : 
                //         task.Result
                //             .Where(lobby => lobby != null)
                //             .Select(lobby => new LobbyItem.LobbyItemData 
                //             { 
                //                 lobbyName = lobby.Name, 
                //                 lobbyId = lobby.Id, 
                //                 playerCount = lobby.MaxPlayers - lobby.AvailableSlots, 
                //                 maxPlayerCount = lobby.MaxPlayers 
                //             }).ToList()
                // };
                // lobbyList.Refresh(lobbyListData);

                yield return new WaitForSeconds(lobbyRefreshInterval);
            }
        }
    }
}