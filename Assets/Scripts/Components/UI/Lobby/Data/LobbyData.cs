using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Components.UI.Lobby
{
    public class LobbyData : MonoBehaviour, UIBehaviour<LobbyData.LobbyDataElement>, IDisplayable
    {
        public struct LobbyDataElement
        {
            public SaveSection.SaveSectionData? saveSectionData;
            public NetworkSection.NetworkSectionData? networkSectionData;
        }
        
        [Header("References")]
        [SerializeField] SaveSection _saveSection;
        [SerializeField] NetworkSection _networkSection;
        [SerializeField] private CanvasGroup _canvasGroup;
        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;
        
        
        public void Refresh(LobbyDataElement dataElement)
        {
            if (dataElement.saveSectionData != null)
                _saveSection.Refresh(dataElement.saveSectionData.Value);
            if (dataElement.networkSectionData != null)
                _networkSection.Refresh(dataElement.networkSectionData.Value);
        }
    }
}