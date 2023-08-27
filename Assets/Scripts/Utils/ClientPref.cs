using System.Collections.Generic;
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
            "I",
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
        
        #endregion
        
        #region Setters
        
        public static void SetNickname(string nickname)
        {
            PlayerPrefs.SetString(NicknameKey, nickname);
        }
        
        #endregion
    }
}