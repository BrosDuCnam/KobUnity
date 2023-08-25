using UnityEngine;

namespace Utils
{
    public static class ClientPref
    {
        // keys
        private const string NicknameKey = "nickname";
        
        #region Getters
        
        public static string GetNickname()
        {
            return PlayerPrefs.GetString(NicknameKey, "");
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