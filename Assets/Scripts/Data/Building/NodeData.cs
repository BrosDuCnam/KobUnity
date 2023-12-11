using System;
using Unity.Netcode;

namespace Data.Building
{
    public struct NodeData : IEquatable<NodeData>, INetworkSerializable
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