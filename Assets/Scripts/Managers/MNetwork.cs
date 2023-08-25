using System;
#if UNITY_EDITOR
using ParrelSync;
#endif
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;
using Utils.Network;

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
