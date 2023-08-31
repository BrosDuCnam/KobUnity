using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Utils;

namespace Components.UI.Game.Inventory
{
    public class InventoryData : IEquatable<InventoryData>, INetworkSerializable
    {
        public List<ItemSlotData> items = new List<ItemSlotData>();

        public bool Equals(InventoryData other)
        {
            return other != null && items == other.items;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            /* ItemSlotData: xxyyyy
             * List<ItemSlotData>: count + ItemSlotData: zxxyyyy
             * 
             */
            
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();

                List<byte> values = new List<byte>();
                reader.ReadValueSafe(out uint count);
                
                for (int i = 0; i < count; i++)
                {
                    reader.ReadValueSafe(out byte value);
                    values.Add(value);
                }

                items = DecodeInventoryData(StringCompressor.DecompressString(Encoding.UTF8.GetString(values.ToArray()))).items;
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                
                byte[] values = Encoding.UTF8.GetBytes(StringCompressor.CompressString(EncodeInventoryData(this)));
                
                writer.WriteValueSafe((uint)values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    writer.WriteValueSafe(values[i]);
                }
            }
        }
        
        public static string EncodeInventoryData(InventoryData data)
        {
            // value : xxyyyy
            // xx : amount
            // yyyy : id
            
            string value = data.items.Count.ToString("00");
            for (int i = 0; i < data.items.Count; i++)
            {
                value += ItemSlotData.EncodeItemSlotData(data.items[i]);
            }
            
            return value;
        }
        
        public static InventoryData DecodeInventoryData(string value)
        {
            // value : xxyyyy
            // xx : amount
            // yyyy : id
            
            InventoryData data = new InventoryData();
            
            int count = int.Parse(value.Substring(0, 2));
            for (int i = 0; i < count; i++)
            {
                data.items.Add(ItemSlotData.DecodeItemSlotData(int.Parse(value.Substring(2 + i * 5, 5))));
            }

            return data;
        }
    }
}