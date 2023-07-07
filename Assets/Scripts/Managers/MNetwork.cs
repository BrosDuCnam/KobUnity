using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MNetwork : NetworkManager
{
    public void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lobby") return;

        if (ClonesManager.IsClone())
        {
            StartClient();
        }
        else
        {
            StartHost();
        }
    }
}
