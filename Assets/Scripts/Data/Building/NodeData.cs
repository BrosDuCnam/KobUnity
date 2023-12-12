using System;
using Interfaces;
using SimpleJSON;
using Unity.Netcode;

namespace Data.Building
{
    public struct NodeData : IEquatable<NodeData>, INetworkSerializable, ISavable
    {
        public int nodeId;

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
            // Some data like orientation, color, etc.
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("nodeId", 0);
            
            return json;
        }

        public void Load(JSONObject json)
        {
            nodeId = json["nodeId"].AsInt;
        }

        #endregion
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();

                reader.ReadValueSafe(out nodeId);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                writer.WriteValueSafe(nodeId);
            }
        }
    }
}