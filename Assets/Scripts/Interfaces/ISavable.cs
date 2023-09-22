using System.Collections.Generic;

namespace Interfaces
{
    public interface ISavable
    {
        public SimpleJSON.JSONObject Save();
        
        public SimpleJSON.JSONObject GetDefaultSave();
        
        public void Load(SimpleJSON.JSONObject json);
    }
}