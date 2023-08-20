using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using Components.UI.Lobby;
#if UNITY_EDITOR
using ParrelSync;
#endif
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class MNetwork : NetworkManager
{
    #region Singleton
    
    public new static MNetwork Singleton { get; private set; }
    
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        
        UnityServices.InitializeAsync();
    }
    
    #endregion

    public enum NetworkAction
    {
        TryJoinLobby,
        FinishJoinLobby,
    }
    
    [Header("Player settings")]
    [SerializeField] private List<string> defaultPlayerNames = new List<string>()
    {
        // Funny nicknames
        "Baby Yoda",
        "Mandalorian",
        "Darth Vader",
        "Darth Maul",
        "Darth Sidious",
        "Obi-Wan Kenobi",
        "Han Solo",
        "Chewbacca",
        "Luke Skywalker",
        "Leia Organa",
        "R2-D2",
        "C-3PO",
        "BB-8",
        "Yoda",
        "Jabba the Hutt",
        "Boba Fett",
        "Jango Fett",
        "Padmé Amidala",
        "Qui-Gon Jinn",
        "Mace Windu",
        "Count Dooku",
        "General Grievous",
        "Kylo Ren",
        "Rey",
        "Poe Dameron",
        "Finn",
        "Captain Phasma",
        "Lando Calrissian",
        "Jyn Erso",
        "Grand Moff Tarkin"
    };

    [Header("Lobby settings")]
    [SerializeField] private string defaultLobbyName = "Default Lobby";
    [SerializeField] private float heartbeatInterval = 15f;
    [SerializeField] private float lobbyPollingInterval = 5f;
    
    [Header("Events")]
    public UnityEvent<Lobby> lobbyUpdated = new();
    public UnityEvent<NetworkAction> onAction;
    
    private Lobby _lobby;
    private float _heartbeatTimer;
    private float _lobbyPollingTimer;

    public bool IsLobbyHost => _lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId;
    public Lobby Lobby => _lobby;
    
    public async void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Lobby")
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                StartClient();
            }
            else
            {
                StartHost();
            }
#endif
            return;
        }
        
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in anonymously as: " + AuthenticationService.Instance.PlayerId);
        };
            
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Update()
    {
        HandleHeartbeat();
        HandleLobbyPolling();
    }
    
    private async void HandleHeartbeat()
    {
        if (_lobby == null) return;
        if (_lobby.HostId != AuthenticationService.Instance.PlayerId) return;
        
        _heartbeatTimer -= Time.deltaTime;
        if (_heartbeatTimer <= 0)
        {
            _heartbeatTimer = heartbeatInterval;

            await LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
        }
    }

    private async void HandleLobbyPolling()
    {
        if (_lobby == null) return;
        
        _lobbyPollingTimer -= Time.deltaTime;
        if (_lobbyPollingTimer <= 0)
        {
            _lobbyPollingTimer = lobbyPollingInterval;

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
            if (lobby == null)
            {
                Debug.Log("Lobby doesn't exist anymore");
                SetLobby(null);
                return;
            }
            
            SetLobby(lobby);
        }
    }

    #region Lobby
    
    private void SetLobby(Lobby lobby)
    {
        bool isSameLobby = _lobby != null && lobby != null && _lobby.Id == lobby.Id;
        
        _lobby = lobby;
        lobbyUpdated.Invoke(lobby);
        
        if (lobby == null) return;
        if (isSameLobby) return;
        if (lobby.Players.All(p => p.Id != AuthenticationService.Instance.PlayerId)) return; // Player is not in lobby
        
        UpdatePlayerOptions options = new UpdatePlayerOptions();
        options.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, GetRandomPlayerName()) }
        };
        
        LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, options);
    }
    
    public async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "code", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: GenerateLobbyCode(),
                            index: DataObject.IndexOptions.S1)
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(defaultLobbyName, 5, options);
            Debug.Log("Created lobby: " + lobby.Id + " with code: " + lobby.LobbyCode);
            
            SetLobby(lobby);
            
            LobbyUI.Singleton.LoadPanel(LobbyUI.Panel.Room);
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<Lobby> JoinLobbyByCode(string code)
    {
        onAction.Invoke(NetworkAction.TryJoinLobby);
        
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>()
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new(QueryFilter.FieldOptions.S1, code, QueryFilter.OpOptions.EQ),
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
                return null;
            }
            
            Lobby lobby = response.Results[0];
            
            lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            SetLobby(lobby);
            onAction.Invoke(NetworkAction.FinishJoinLobby);
            
            return lobby;
        } catch (Exception e)
        {
            Debug.LogError(e);
            onAction.Invoke(NetworkAction.FinishJoinLobby);
            return null;
        }
    }
    
    #endregion
    
    public static string GenerateLobbyCode()
    {
        const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789"; // No I, L, 1, O, 0 to avoid confusion
        return new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
    
    public string GetRandomPlayerName()
    {
        return defaultPlayerNames[new Random().Next(defaultPlayerNames.Count)];
    }
}
