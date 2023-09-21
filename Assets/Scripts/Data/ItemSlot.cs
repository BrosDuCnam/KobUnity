using System;
using Interfaces;
using SimpleJSON;
using Unity.Netcode;

namespace Components.Data
{
    public struct ItemSlot : IEquatable<ItemSlot>, INetworkSerializable, ISavable
    {
        #region IEquatable Implementation
        
        public bool Equals(ItemSlot other)
        {
            return amount == other.amount && id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemSlot other && Equals(other);
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
                
                reader.ReadValueSafe(out amount);
                reader.ReadValueSafe(out id);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                
                writer.WriteValueSafe(amount);
                writer.WriteValueSafe(id);
            }
        }
        
        #endregion

        #region ISavable Implementation
        
        public JSONObject Save()
        {
            JSONObject json = new JSONObject();
            json.Add("amount", amount);
            json.Add("id", id);
            
            return json;
        }

        public JSONObject GetDefaultSave()
        {
            JSONObject json = new JSONObject();
            json.Add("amount", 0);
            json.Add("id", 0);
            
            return json;
        }

        public void Load(JSONObject json)
        {
            amount = json["amount"].AsInt;
            id = json["id"].AsInt;
        }

        #endregion
        
        public int amount;
        public int id;
            
        public ItemSlot Add(int amountAdded)
        {
            return new ItemSlot()
            {
                amount = amount + amountAdded,
                id = id
            };
        }
        public (ItemSlot, ItemSlot) Split(int amountSplit = -1)
        {
            int leftAmount = amountSplit == -1 ? amount / 2 : amountSplit;
            int rightAmount = amount - leftAmount;
                
            return (new ItemSlot()
            {
                amount = leftAmount,
                id = id
            }, new ItemSlot()
            {
                amount = rightAmount,
                id = id
            });
        }

        public bool IsVoid => amount == 0 || id == 0;
        public static ItemSlot Void => new ItemSlot()
        {
            amount = 0,
            id = 0
        };
            
        public static bool operator ==(ItemSlot a, ItemSlot b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ItemSlot a, ItemSlot b)
        {
            return !(a == b);
        }
        
        public override string ToString()
        {
            return $"ItemSlot({id}, {amount})";
        }
    }
}