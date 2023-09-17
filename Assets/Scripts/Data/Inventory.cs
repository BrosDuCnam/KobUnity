using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string ToString()
        {
            if (items == null || items.Count == 0) return "Empty inventory";
            
            return new StringBuilder()
                .AppendLine($"Inventory: {items.Count} items")
                .AppendLine("Items:")
                .AppendLine(items.Select(item => item.ToString()).Aggregate((a, b) => a + "\n" + b))
                .ToString();
        }
    }
}