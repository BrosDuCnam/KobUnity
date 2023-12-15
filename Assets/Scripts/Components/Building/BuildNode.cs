using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Components.Building
{
    public class BuildNode : MonoBehaviour
    {
        public int nodeId;
        public BaseBuild build;

        public enum BuildType
        {
            Platform,
        }
        
        [SerializeField] public BuildType type;
        [SerializeField] public SerializedDictionary<int, BuildAnchor> anchors; // anchorsId, anchor
        
        [SerializeField] public SerializedDictionary<int, BuildNode> children; // anchorsId, anchor
        
        public bool CheckPermissionToBuild(int anchorId)
        {
            return true;
        }
        
        public static BuildNode Instantiate(BuildType type, Transform parent, Vector3 position, BaseBuild build, int id)
        {
            var node = Instantiate(UResources.GetBuildNodePrefab(type), parent);
            node.transform.localPosition = position;
            
            node.nodeId = id;
            node.build = build;
            
            // Link other node to this one
            for (int i = 0; i < node.anchors.Count; i++)
            {
                BuildAnchor anchor = node.anchors.Values.ToList()[i];
                int anchorId = node.anchors.Keys.ToList()[i];
                
                BuildType anchorType = anchor.child.type;
                List<BuildNode> others = build.nodes.FindAll(x => x!= null && x.type == anchorType);

                foreach (BuildNode other in others)
                {
                    if (other.transform.position == anchor.child.transform.position)
                    {
                        node.children.Add(anchorId, other);
                    }
                }
            }
            
            // Link this node to other
            foreach (BuildNode other in build.nodes)
            {
                if (other == null) continue;
                
                for (int i = 0; i < other.anchors.Count; i++)
                {
                    BuildAnchor anchor = other.anchors.Values.ToList()[i];
                    int anchorId = other.anchors.Keys.ToList()[i];
                    
                    if (anchor.child.transform.position == node.transform.position)
                    {
                        other.children.Add(anchorId, node);
                    }
                }
            }

            build.nodes.Add(node);            
            return node;
        }
        
        public void Destroy()
        {
            // Remove this node from other nodes
            foreach (BuildNode other in build.nodes)
            {
                foreach (var child in other.children)
                {
                    if (child.Value.nodeId == nodeId)
                    {
                        other.children.Remove(child.Key);
                        break;
                    }
                }
            }
            
            // Remove this node from build
            build.nodes.Remove(this);
            
            Destroy(gameObject);
        }
    }
}