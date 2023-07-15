using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyItem : UIBehaviour<LobbyItem.LobbyItemData>
    {
        [Header("UI References")]
        [SerializeField] private TMPro.TMP_Text lobbyNameText;
        [SerializeField] private TMPro.TMP_Text playerCountText;

        public struct LobbyItemData
        {
            public string lobbyName;
            public string lobbyId;
            public int playerCount;
            public int maxPlayerCount;
        }
        
        public override void Refresh(LobbyItemData data)
        {
            lobbyNameText.text = data.lobbyName;
            playerCountText.text = $"{data.playerCount}/{data.maxPlayerCount}";
        }
    }
}