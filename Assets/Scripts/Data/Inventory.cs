using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Utils;

namespace Components.Data
{
    public struct Inventory : IEquatable<Inventory>
    {
        public List<ItemSlot> items;


        public bool Equals(Inventory other)
        {
            return Equals(items, other.items);
        }

        public override bool Equals(object obj)
        {
            return obj is Inventory other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (items != null ? items.GetHashCode() : 0);
        }
    }
}