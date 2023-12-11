using Interfaces;
using SimpleJSON;
using UnityEngine;

namespace Components.Building
{
    public class BaseBuild : MonoBehaviour, ISavable
    {
        
        
        #region Save

        public JSONObject Save()
        {
            throw new System.NotImplementedException();
        }

        public JSONObject GetDefaultSave()
        {
            throw new System.NotImplementedException();
        }

        public void Load(JSONObject json)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}