using System;
using Components.Building;
using Interfaces;
using SimpleJSON;
using Unity.Netcode;

namespace Data.Building
{
    public struct NodeData : IEquatable<NodeData>, INetworkSerializable, ISavable
    {
        public int nodeId;
        
        public bool isRoot;
        public BuildNode.BuildType type;
        
        // Some data like orientation, color, etc.


        #region IEquatable Implementation

        public bool Equals(NodeData other)
        {
            return nodeId == other.nodeId;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return nodeId;
        }

        #endregion

        #region ISavable Implementation

        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            json.Add("nodeId", nodeId);
            json.Add("isRoot", isRoot);
            json.Add("type", (int) type);
            // Some data like orientation, color, etc.
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("nodeId", 0);
            json.Add("isRoot", false);
            json.Add("type", (int) BuildNode.BuildType.Platform);
            
            return json;
        }

        public void Load(JSONObject json)
        {
            nodeId = json["nodeId"].AsInt;
            
            isRoot = json["isRoot"].AsBool;
            type = (BuildNode.BuildType) json["type"].AsInt;
        }

        #endregion
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();

                reader.ReadValueSafe(out nodeId);
                
                reader.ReadValueSafe(out isRoot);
                reader.ReadValueSafe(out type);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                writer.WriteValueSafe(nodeId);
                
                writer.WriteValueSafe(isRoot);
                writer.WriteValueSafe(type);
            }
        }
    }
}