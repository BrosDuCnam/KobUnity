using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils.Network;

public class MNetworkHandler : MonoBehaviour
{
    #region Singleton
    
    public static MNetworkHandler Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        UnityServices.InitializeAsync();
    }
    
    #endregion
    
    public readonly AuthHandler authHandler = new();
    public readonly LobbyHandler lobbyHandler = new();
    public readonly RelayHandler relayHandler = new();

#if UNITY_EDITOR
    [SerializeField] public UnityEditor.SceneAsset onlineScene;
    [SerializeField] public UnityEditor.SceneAsset offlineScene;
#endif
    
    [SerializeField] private string onlineSceneName;
    [SerializeField] private string offlineSceneName;
    
    [HideInInspector] public bool isHost = false;
    [HideInInspector] public bool isClient = false;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (onlineScene != null) onlineSceneName = onlineScene.name;
        if (offlineScene != null) offlineSceneName = offlineScene.name;
#endif
    }

    
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        TrySignIn();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Auto start host if not in lobby scene (for testing)
        if (NetworkManager.Singleton != null &&
            SceneManager.GetActiveScene().name != "Lobby")
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                NetworkManager.Singleton.StartHost();
            }
#endif
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;
        if (NetworkManager.Singleton == null) return;
        
        if (NetworkManager.Singleton.IsListening) return; // Already listening
        if (relayHandler.serverData == null) return; // No relay server data

        if (isHost && isClient)
        {
            Debug.LogError("Host and client at the same time, this should not happen");
            return;
        }
        
        NetworkManager.Singleton.OnClientStopped += (_) => Disconnect();
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayHandler.serverData.Value);
        if (isHost) NetworkManager.Singleton.StartHost();
        else if (isClient) NetworkManager.Singleton.StartClient();
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
    
    public async Task<string> StartHost()
    {
        string code = await relayHandler.CreateRelay();
        SceneManager.LoadScene(onlineSceneName);
        
        return code;
    }
    
    public async void JoinRelay(string relayCode)
    {
        await relayHandler.JoinRelay(relayCode);
        SceneManager.LoadScene(onlineSceneName);
    }
    
    public void Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            // TODO : if host disconnect everyone
            
            if (NetworkManager.Singleton.IsListening) NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene(offlineSceneName);
        
        Cursor.lockState = CursorLockMode.None;
    }
}
