using System.Collections.Generic;

namespace Data.Building
{
    public struct Node
    {
        public int id;
        public NodeData data;
        public List<NodeAnchor> anchors;
    }
}