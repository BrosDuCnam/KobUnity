using System.Collections.Generic;
using Data.Building;

namespace Utils
{
    public static class MermaidGenerator
    {
        public static string GenerateBuild(Build build)
        {
            string mermaid = "graph LR\n";
            
            // Setup nodes
            foreach (var node in build.nodes)
            {
                mermaid += $"    {node.id}[{node.id} \n type: platform]\n";
            }
            
            List<(int, int)> links = new List<(int, int)>();
            
            // Setup links
            foreach (var node in build.nodes)
            {
                foreach (var anchor in node.anchors)
                {
                    if (links.Contains((node.id, anchor.childId))) continue;
                    else if (links.Contains((anchor.childId, node.id))) continue;
                    
                    List<string> description = new List<string>();
                    description.Add($"anchorId: {anchor.anchorId}");
                    
                    foreach (var otherNode in build.nodes)
                    {
                        if (otherNode.id == anchor.childId)
                        {
                            foreach (var otherAnchor in otherNode.anchors)
                            {
                                if (otherAnchor.childId == node.id)
                                { 
                                    description.Add($"anchorId: {otherAnchor.anchorId}");
                                    break;
                                }
                            }
                        }
                    }
                    
                    if (description.Count > 1)
                    {
                        mermaid += $"    {node.id}<--{string.Join(", ", description)}--> {anchor.childId}\n";
                    }
                    else
                    {
                        mermaid += $"    {node.id}--{description[0]}--> {anchor.childId}\n";
                    }
                    
                    links.Add((node.id, anchor.childId));
                }
            }
            
            return mermaid;
        }
    }
}