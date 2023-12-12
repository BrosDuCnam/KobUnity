using System.Collections.Generic;
using Interfaces;
using SimpleJSON;

namespace Data.Building
{
    public struct Node : ISavable
    {
        public int id;
        public NodeData data;
        public List<NodeAnchor> anchors;
        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            json.Add("id", id);
            json.Add("data", data.Save());
            
            JSONArray anchorsJson = new JSONArray();
            foreach (var anchor in anchors)
            {
                anchorsJson.Add(anchor.Save());
            }
            
            json.Add("anchors", anchorsJson);
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("id", 0);
            json.Add("data", data.GetDefaultSave());
            
            JSONArray anchorsJson = new JSONArray();
            anchorsJson.Add(new NodeAnchor().GetDefaultSave());
            
            json.Add("anchors", anchorsJson);
            
            return json;
        }

        public void Load(JSONObject json)
        {
            id = json["id"].AsInt;
            data.Load(json["data"].AsObject);
            
            anchors = new List<NodeAnchor>();
            foreach (var anchor in json["anchors"].AsArray)
            {
                NodeAnchor nodeAnchor = new NodeAnchor();
                nodeAnchor.Load(anchor.Value.AsObject);
                anchors.Add(nodeAnchor);
            }
        }
    }
}