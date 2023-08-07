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
    
    public enum ActionEnum
    {
        TryJoinLobby,
        FinishJoinLobby,
    }
    
    [Header("Lobby settings")]
    [SerializeField] private string defaultLobbyName = "Default Lobby";
    [SerializeField] private float heartbeatInterval = 15f;
    
    public UnityEvent<ActionEnum> onAction = new UnityEvent<ActionEnum>();
    
    private Lobby _lobby;
    private float _heartbeatTimer;
    
    public bool IsHost => _lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId;
    
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
    
    public async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(defaultLobbyName, 5, options);
            Debug.Log("Created lobby: " + lobby.Id + " with code: " + lobby.LobbyCode);
            
            _lobby = lobby;
            
            LobbyUI.Singleton.LoadPanel(LobbyUI.Panel.Room);
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<List<Lobby>> GetLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>()
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                },
                Order = new List<QueryOrder>()
                {
                    new(false, QueryOrder.FieldOptions.Created),
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            
            Debug.Log("Found :" + string.Join(", ", response.Results.Select(x => x.Id)));
            
            return response.Results;

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        onAction.Invoke(ActionEnum.TryJoinLobby);
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            
            onAction.Invoke(ActionEnum.FinishJoinLobby);
            Debug.Log("Joined lobby: " + lobby.Id);
            
            _lobby = lobby;
        } catch (Exception e)
        {
            onAction.Invoke(ActionEnum.FinishJoinLobby);
            Debug.LogError(e);
        }
    }
}
