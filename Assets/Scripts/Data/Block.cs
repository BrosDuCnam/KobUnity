using Unity.Netcode;

namespace Components.Data
{
    public struct Block : INetworkSerializable
    {
        public int id;
        public byte rotation; // 0-3 (0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°)
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out id);
                reader.ReadValueSafe(out rotation);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(id);
                writer.WriteValueSafe(rotation);
            }
        }
    }
}