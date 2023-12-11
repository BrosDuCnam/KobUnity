using System.Collections.Generic;

namespace Data.Building
{
    public struct Build
    {
        public List<Node> nodes;
        
        public static List<BuildChange> Compare(Build oldBuild, Build newBuild)
        {
            List<BuildChange> changes = new List<BuildChange>();
            
            // Check for removed nodes
            foreach (var oldNode in oldBuild.nodes)
            {
                if (newBuild.nodes.FindIndex(n => n.id == oldNode.id) >= 0) continue;
                
                changes.Add(new BuildChange()
                {
                    added = false,
                    data = oldNode.data
                });
            }
            
            // Check for added nodes
            foreach (var newNode in newBuild.nodes)
            {
                if (oldBuild.nodes.FindIndex(n => n.id == newNode.id) >= 0) continue;
                
                changes.Add(new BuildChange()
                {
                    added = true,
                    data = newNode.data
                });
            }

            return changes;
        }
    }
    
    public struct BuildChange
    {
        // Struct to store if node was added or removed
        public bool added;
        public NodeData data;
    }
}