using System;
using Unity.Netcode;

namespace Data.Building
{
    public struct NodeAnchor : IEquatable<NodeAnchor>, INetworkSerializable
    {
        public int nodeId;
        public int anchorId;
        public int childId;

        #region IEquatable Implementation
        
        public bool Equals(NodeAnchor other)
        {
            return nodeId == other.nodeId && anchorId == other.anchorId && childId == other.childId;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeAnchor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nodeId, anchorId, childId);
        }
        #endregion
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                
                reader.ReadValueSafe(out nodeId);
                reader.ReadValueSafe(out anchorId);
                reader.ReadValueSafe(out childId);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                
                writer.WriteValueSafe(nodeId);
                writer.WriteValueSafe(anchorId);
                writer.WriteValueSafe(childId);
            }
        }
    }
}