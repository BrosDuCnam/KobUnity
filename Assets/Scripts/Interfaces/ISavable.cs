using System.Collections.Generic;

namespace Interfaces
{
    public interface ISavable
    {
        public List<ISavable> Children { get; set; }
        
        public SimpleJSON.JSONObject Save();
        
        public void Load(SimpleJSON.JSONObject json);
    }
}