using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Managers
{
    public class MLobby : MonoBehaviour
    {
        
        public Lobby currentLobby;
        
        [Header("Settings")]
        [SerializeField] private float heartbeatInterval = 15f;
        
        
        private float _heartbeatTimer;
        
        private async void Start()
        { 
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in : " + AuthenticationService.Instance.PlayerId);
            };
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
        }

        private void Update()
        {
            HandleHeartbeat();
        }
        
        public async void HandleHeartbeat()
        {
            if (currentLobby == null) return;

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0)
            {
                _heartbeatTimer = heartbeatInterval;
                await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
        
        public async void StartLobby()
        {
            var lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", 5);
            
            Debug.Log("Lobby created : " + lobby.Id);
            currentLobby = lobby;
        }

        public async Task<List<Lobby>> GetLobbies()
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            
            Debug.Log("Lobbies found : " + response.Results.Count);
            return response.Results;
        }
    }
}