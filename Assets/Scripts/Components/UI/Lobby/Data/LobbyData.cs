﻿using System;
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
            public SaveSection.SaveSectionData saveSectionData;
            public NetworkSection.NetworkSectionData? networkSectionData;
        }
        
        [Header("References")]
        [SerializeField] SaveSection _saveSection;
        [SerializeField] NetworkSection _networkSection;
        [SerializeField] private CanvasGroup _canvasGroup;
        CanvasGroup IDisplayable.CanvasGroup => _canvasGroup;
        
        
        public void Refresh(LobbyDataElement newData)
        {
            _saveSection.Refresh(newData.saveSectionData);
            
            // if newtworkSectionData is null, set it to a new NetworkSectionData with online = false
            newData.networkSectionData ??= new NetworkSection.NetworkSectionData
            {
                online = false,
            };
            
            _networkSection.Refresh(newData.networkSectionData.Value);
        }
    }
}