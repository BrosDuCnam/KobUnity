using System;
using Unity.Collections;
using Unity.Netcode;

namespace Components.UI.Game.Inventory
{
    public struct ItemSlotData : IEquatable<ItemSlotData>, INetworkSerializable
    {
        #region IEquatable Implementation
        
        public bool Equals(ItemSlotData other)
        {
            return amount == other.amount && id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemSlotData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(amount, id);
        }

        #endregion

        #region INetworkSerializable Implementation

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // value : xxyyyy
            // xx : amount
            // yyyy : id
            
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                
                reader.ReadValueSafe(out int intValue);
                ItemSlotData data = DecodeItemSlotData(intValue);
                amount = data.amount;
                id = data.id;
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                
                writer.WriteValueSafe(EncodeItemSlotData(this));
            }
        }
        
        public static string EncodeItemSlotData(ItemSlotData data)
        {
            // value : xxyyyy
            // xx : amount
            // yyyy : id
            
            return data.amount.ToString("00") + data.id.ToString("0000");
        }
        
        public static ItemSlotData DecodeItemSlotData(int value)
        {
            // value : xxyyyy
            // xx : amount
            // yyyy : id
            
            return new ItemSlotData()
            {
                amount = int.Parse(value.ToString().Substring(0, 2)),
                id = int.Parse(value.ToString().Substring(2))
            };
        }
        
        #endregion
        
        public int amount;
        public int id;
            
        public ItemSlotData Add(int amountAdded)
        {
            return new ItemSlotData()
            {
                amount = amount + amountAdded,
                id = id
            };
        }
        public (ItemSlotData, ItemSlotData) Split(int amountSplit = -1)
        {
            int leftAmount = amountSplit == -1 ? amount / 2 : amountSplit;
            int rightAmount = amount - leftAmount;
                
            return (new ItemSlotData()
            {
                amount = leftAmount,
                id = id
            }, new ItemSlotData()
            {
                amount = rightAmount,
                id = id
            });
        }

        public bool IsVoid => amount == 0 || string.IsNullOrEmpty(id.ToString());
        public static ItemSlotData Void => new ItemSlotData()
        {
            amount = 0,
            id = 0
        };
            
        public static bool operator ==(ItemSlotData a, ItemSlotData b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ItemSlotData a, ItemSlotData b)
        {
            return !(a == b);
        }
    }
}