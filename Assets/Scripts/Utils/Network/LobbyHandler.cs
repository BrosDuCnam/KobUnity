﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Utils.Network
{
    public class LobbyHandler
    {
        private Lobby _lobby;
        private const string KEY_CODE = "lobbyCode";
        private const string KEY_STARTGAME = "startGame";
        private const string KEY_PLAYERNAME = "name";
        
        private const int HEARTBEAT_INTERVAL = 5;
        private float _lastHeartbeat = 0;
        private bool CanHeartbeat => Time.time - _lastHeartbeat > HEARTBEAT_INTERVAL;
        
        private const int POLL_INTERVAL = 2;
        private float _lastPoll = 0;
        private bool CanPoll => Time.time - _lastPoll > POLL_INTERVAL;
        
        // Events
        public UnityEvent<Lobby> onLobbyChanged = new ();
        
        public Lobby Lobby
        {
            get => _lobby;
            set
            {
                _lobby = value;
                onLobbyChanged?.Invoke(_lobby);
            }
        }
        public bool IsLobbyOwner => Lobby != null && Lobby.HostId == AuthenticationService.Instance.PlayerId;
        
        #region Lobby Handlers
        
        public async Task CreateLobby()
        {
            try
            {
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KEY_CODE, new DataObject(
                                visibility: DataObject.VisibilityOptions.Public,
                                value: GenerateLobbyCode(),
                                index: DataObject.IndexOptions.S1)
                        },
                        {
                            KEY_STARTGAME, new DataObject(
                                visibility: DataObject.VisibilityOptions.Member,
                                value: "0",
                                index: DataObject.IndexOptions.S2)
                        }
                    }
                };

                // TODO: fill default lobby name
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("", 5, options);
                await ApplyNickname(lobby);
            
                Lobby = lobby;
            } catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public async Task JoinLobbyByCode(string code)
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>()
                    {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT), // Check lobby is not full
                        new(QueryFilter.FieldOptions.S1, code, QueryFilter.OpOptions.EQ), // Check lobby code
                    },
                    Order = new List<QueryOrder>()
                    {
                        new(false, QueryOrder.FieldOptions.Created),
                    }
                };
            
                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            
                if (response.Results.Count == 0)
                {
                    Debug.Log("No lobby found with code: " + code);
                    return;
                }
            
                Lobby lobby = response.Results[0];
            
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                await ApplyNickname(lobby);
            
                Lobby = lobby;
            } 
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        #endregion

        #region Events

        public void OnUpdate()
        {
            if (Lobby == null) return;
            
            if (CanHeartbeat && IsLobbyOwner) Heartbeat();
            if (CanPoll) Poll();
        }

        private async Task Heartbeat()
        {
            _lastHeartbeat = Time.time;
            
            await LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
        }

        private async Task Poll()
        {
            _lastPoll = Time.time;
            
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
            
            if (lobby == null)
            {
                Debug.Log("Lobby doesn't exist anymore");
                
                Lobby = null;
                return;
            }

            if (lobby.Data[KEY_STARTGAME].Value != "0")
            {
                Debug.Log("Lobby is starting game");
                if (!IsLobbyOwner)
                {
                    JoinRelay(lobby.Data[KEY_STARTGAME].Value);
                }

                Lobby = null; // Reset lobby so we don't get stuck in a loop
                return;
            }
            
            Lobby = lobby;
        }
        
         #endregion
        
        #region Helper
        
        private async Task ApplyNickname(Lobby lobby)
        {
            try
            {
                if (lobby == null) return;
                if (lobby.Players == null) return;
                if (lobby.Players.All(p => p.Id != AuthenticationService.Instance.PlayerId)) return; // Not in lobby
            
                UpdatePlayerOptions options = new UpdatePlayerOptions();
                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    { KEY_PLAYERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ClientPref.GetNickname()) }
                };
            
                await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
        
        #region Utils
        
        public static string GenerateLobbyCode()
        {
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789"; // No I, L, 1, O, 0 to avoid confusion
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
        
        #endregion
    }
}