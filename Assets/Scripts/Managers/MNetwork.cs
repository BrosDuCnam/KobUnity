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
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utils.Network;
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
    
    public readonly AuthHandler authHandler = new();
    public readonly LobbyHandler lobbyHandler = new();
    public readonly RelayHandler relayHandler = new();

    public void Start()
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
        
        TrySignIn();
    }
    
    private void Update()
    {
        lobbyHandler.OnUpdate();
    }
    
    private async void TrySignIn()
    {
        try
        {
            string profile = "default";
            
#if UNITY_EDITOR
            if (ClonesManager.IsClone()) profile = "clone";
#endif
            
            
            var unityAuthenticationInitOptions = authHandler.GenerateAuthenticationOptions(profile);

            await authHandler.InitializeAndSignInAsync(unityAuthenticationInitOptions);
            
            // TODO: OnAuthSignIn();
        }
        catch (Exception)
        {
            // TODO: OnSignInFailed();
        }
    }
    
    public void JoinRelay(string relayCode)
    {
        relayHandler.JoinRelay(relayCode);
    }
}
