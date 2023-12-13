using System.Collections.Generic;
using Interfaces;
using SimpleJSON;

namespace Data.Building
{
    public struct Build : ISavable
    {
        public List<Node> nodes;
        
        public static List<BuildChange> Compare(Build oldBuild, Build newBuild)
        {
            List<BuildChange> changes = new List<BuildChange>();
            
            // Check for removed nodes
            foreach (var oldNode in oldBuild.nodes)
            {
                if (newBuild.nodes.FindIndex(n => n.id == oldNode.id && n.data != null) >= 0) continue;
                if (oldNode.data == null) continue;
                
                changes.Add(new BuildChange()
                {
                    added = false,
                    data = oldNode.data.Value
                });
            }
            
            // Check for added nodes
            foreach (var newNode in newBuild.nodes)
            {
                if (oldBuild.nodes.FindIndex(n => n.id == newNode.id && n.data != null) >= 0) continue;
                if (newNode.data == null) continue;
                
                changes.Add(new BuildChange()
                {
                    added = true,
                    data = newNode.data.Value
                });
            }

            return changes;
        }

        #region ISavable
        
        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            
            JSONArray nodesJson = new JSONArray();
            foreach (var node in nodes)
            {
                nodesJson.Add(node.Save());
            }
            json.Add("nodes", nodesJson);
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            
            JSONArray nodesJson = new JSONArray();
            nodesJson.Add(new Node().GetDefaultSave());
            json.Add("nodes", nodesJson);
            
            return json;
        }

        public void Load(JSONObject json)
        {
            nodes = new List<Node>();
            foreach (var node in json["nodes"].AsArray)
            {
                Node nodeData = new Node();
                nodeData.Load(node.Value.AsObject);
                nodes.Add(nodeData);
            }
        }
        
        #endregion
    }
    
    public struct BuildChange
    {
        // Struct to store if node was added or removed
        public bool added;
        public NodeData data;
    }
}