using System.Collections.Generic;
using ParrelSync;
using UnityEngine;

namespace Utils
{
    public static class ClientPref
    {
        // keys
        private const string NicknameKey = "nickname";
        
        // default nicknames
        private static readonly List<string> DefaultNicknames = new List<string>
        {
            "Player",
            "Guest",
            "Stranger",
            "You",
            "Me",
            "Myself",
            "Unknown",
            "Anonymous",
            "Anon",
            "NoName",
            "NoNickname",
        };
        
        #region Getters
        
        public static string GetNickname()
        {
            if (!PlayerPrefs.HasKey(NicknameKey))
            {
                PlayerPrefs.SetString(NicknameKey, DefaultNicknames[Random.Range(0, DefaultNicknames.Count)]);
            }
            return PlayerPrefs.GetString(NicknameKey);
        }
        
        public static string GetUUID()
        {
            string defaultUuid = SystemInfo.deviceUniqueIdentifier;
            string result;
            
            if (ClonesManager.IsClone())
            {
                string cloneId = "clone_" + ClonesManager.GetCurrentProject().name[^1];
                result = PlayerPrefs.GetString("uuid_" + cloneId, cloneId + "_" + defaultUuid);
            }
            else
            {
                result = PlayerPrefs.GetString("uuid", defaultUuid);
            }
            
            return result;
        }
        
        #endregion
        
        #region Setters
        
        public static void SetNickname(string nickname)
        {
            PlayerPrefs.SetString(NicknameKey, nickname);
        }
        
        #endregion
    }
}