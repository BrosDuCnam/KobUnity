using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyList : UIBehaviour<LobbyList.LobbyListData>
    {
        public struct LobbyListData
        {
            public List<LobbyItem.LobbyItemData> lobbies;
        }

        public override void Refresh(LobbyListData data)
        {
            UIPooling.Refresh<LobbyItem, LobbyItem.LobbyItemData>(data.lobbies, MLobby.Instance.lobbyItemPrefab, transform);
        }
    }
}