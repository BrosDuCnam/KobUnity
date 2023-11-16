using System;
using System.Collections.Generic;
using Components.Building;
using Unity.Collections;
using Unity.Netcode;

namespace Components.Data
{
    public struct GraphNodeBuild
    {
        public List<BlockNode> nodes;
        
        public void AddNode(BlockNode node)
        {
            nodes.Add(node);
        }
    }
    
    public struct BlockNode : INetworkSerializable, IEquatable<BlockNode>
    {
        public const int MaxChildren = 50; // Ajustez selon vos besoins
        public const int MaxParents = 50;

        public int ID;
        public BlockType type;

        public bool isRoot;
        public FixedString64Bytes children;
        public FixedString64Bytes parents;


        public static List<(int anchor, int blockId)> GetAnchorData(FixedString64Bytes data)
        {
            string[] children = data.ToString().Split(',');
            if (children.Length == 0) return null;
            if (children.Length % 2 != 0) throw new Exception("Invalid children string");
            
            List<(int anchor, int blockId)> result = new List<(int anchor, int blockId)>();
            for (int i = 0; i < children.Length; i += 2)
            {
                result.Add((int.Parse(children[i]), int.Parse(children[i + 1])));
            }
            
            return result;
        }
        
        public FixedString64Bytes GetAnchorString(List<(int anchor, int blockId)> data)
        {
            string result = "";
            for (int i = 0; i < data.Count; i++)
            {
                result += $"{data[i].anchor},{data[i].blockId}";
                if (i != data.Count - 1) result += ",";
            }

            return new FixedString64Bytes(result);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();

                reader.ReadValueSafe(out ID);
                reader.ReadValueSafe(out type);
                reader.ReadValueSafe(out isRoot);

                reader.ReadValueSafe(out int childrenCount);
                List<(int anchor, int blockId)> children = new List<(int anchor, int blockId)>();
                for (int i = 0; i < childrenCount; i++)
                {
                    int anchor = 0;
                    reader.ReadValueSafe(out anchor);
                    int blockId = 0;
                    reader.ReadValueSafe(out blockId);
                    children.Add((anchor, blockId));
                }

                this.children = GetAnchorString(children);

                reader.ReadValueSafe(out int parentsCount);
                List<(int anchor, int blockId)> parents = new List<(int anchor, int blockId)>();
                for (int i = 0; i < parentsCount; i++)
                {
                    int anchor = 0;
                    reader.ReadValueSafe(out anchor);
                    int blockId = 0;
                    reader.ReadValueSafe(out blockId);
                    parents.Add((anchor, blockId));
                }

                this.parents = GetAnchorString(parents);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                writer.WriteValueSafe(ID);
                writer.WriteValueSafe(type);
                writer.WriteValueSafe(isRoot);

                List<(int anchor, int blockId)> children = GetAnchorData(this.children);
                writer.WriteValueSafe(children.Count);
                for (int i = 0; i < children.Count; i++)
                {
                    writer.WriteValueSafe(children[i].anchor);
                    writer.WriteValueSafe(children[i].blockId);
                }

                List<(int anchor, int blockId)> parents = GetAnchorData(this.parents);
                writer.WriteValueSafe(parents.Count);
                for (int i = 0; i < parents.Count; i++)
                {
                    writer.WriteValueSafe(parents[i].anchor);
                    writer.WriteValueSafe(parents[i].blockId);
                }
            }
        }

        public bool Equals(BlockNode other)
        {
            if (ID != other.ID || type != other.type || isRoot != other.isRoot)
                return false;
            
            if (children != other.children || parents != other.parents)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, type, isRoot, children, parents);
        }
    }
}