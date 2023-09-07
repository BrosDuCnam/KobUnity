using System.Collections.Generic;

namespace Interfaces
{
    public interface ISavable
    {
        public List<ISavable> Children { get; set; }
        
        public SimpleJSON.JSONObject Save();

        public static SimpleJSON.JSONObject GetDefaultSave()
        {
            // Implement this in your class
            // I need to throw an exception here because I can't make this method abstract
#if UNITY_EDITOR
            throw new System.NotImplementedException();
#else
            return null;
#endif
        }
        
        public void Load(SimpleJSON.JSONObject json);
    }
}