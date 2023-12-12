using System.Collections.Generic;
using System.Linq;
using Data.Building;
using Interfaces;
using Network.Data;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Components.Building
{
    public class BaseBuild : MonoBehaviour, ISavable
    {
        public List<BuildNode> nodes = new List<BuildNode>();
        
        public BuildingData buildingData;

        private void OnBuildChange(List<BuildChange> changes)
        {
            foreach (var change in changes)
            {
                ApplyChange(change);
            }
        }
        
        private void ApplyChange(BuildChange change)
        {
            if (change.added)
            {
                if (buildingData.Value.nodes.Count == 1)
                {
                    // First node
                    BuildNode.Instantiate(BuildNode.BuildType.platform, transform, Vector3.zero, this, change.data.nodeId);
                }
                
                bool found = false;
                
                // Find anchor
                foreach (Node node in buildingData.Value.nodes)
                {
                    foreach (NodeAnchor nodeAnchor in node.anchors)
                    {
                        if (nodeAnchor.childId != change.data.nodeId) continue;

                        foreach (BuildNode buildNode in nodes)
                        {
                            if (buildNode != null && buildNode.nodeId != node.id) continue;
                            
                            foreach (var buildAnchor in buildNode.anchors)
                            {
                                if (buildAnchor.Key != nodeAnchor.anchorId) continue;
                                
                                buildAnchor.Value.Build(change.data.nodeId);
                                
                                // TODO: Apply other modifiers (orientation, etc.)
                                
                                found = true;
                                break;
                            }
                            
                            if (found) break;
                        }
                        
                        if (found) break;
                    }
                    
                    if (found) break;
                }
            }
            else
            {
                // Find build node
                foreach (BuildNode buildNode in nodes)
                {
                    if (buildNode.nodeId != change.data.nodeId) continue;
                    
                    buildNode.Destroy();
                }
            }
        }

        private void Refresh(Build newBuild)
        {
            // TODO
        }

        private void OnEnable()
        {
            buildingData.OnBuildChanged.AddListener(OnBuildChange);
            
            buildingData.OnNetworkSpawned.AddListener(() =>
            {
                // Add first node
                buildingData.AddNode(new NodeData() { nodeId = GetRandomId() }, new List<NodeAnchor>());
                
                
                Refresh(buildingData.Value);
            });
        }

        public Node Build(BuildAnchor anchorParent)
        {
            BuildNode.BuildType type = anchorParent.child.type;
            var node = Instantiate(UResources.GetBuildNodePrefab(type), transform);
            node.transform.position = anchorParent.child.transform.position;
            
            Node nodeData = new Node
            {
                id = GetRandomId(),
                anchors = new List<NodeAnchor>()
            };
            nodeData.data = new NodeData
            {
                nodeId = nodeData.id,
                // TODO: Add other data
            };
            
            List<NodeAnchor> anchors = new List<NodeAnchor>();

            // Link other node to this one
            for (int i = 0; i < node.anchors.Count; i++)
            {
                BuildAnchor anchor = node.anchors.Values.ToList()[i];
                int anchorId = node.anchors.Keys.ToList()[i];
                
                BuildNode.BuildType anchorType = anchor.child.type;
                List<BuildNode> others = nodes.FindAll(x => x != null && x.type == anchorType);

                foreach (BuildNode other in others)
                {
                    if (other.transform.position == anchor.child.transform.position && node.CheckPermissionToBuild(anchorId))
                    {
                        NodeAnchor nodeAnchor = new NodeAnchor
                        {
                            nodeId = nodeData.id,
                            anchorId = anchor.AnchorId,
                            childId = other.nodeId,
                        };
                        
                        anchors.Add(nodeAnchor);
                        nodeData.anchors.Add(nodeAnchor);
                    }
                }
            }
            
            // Link this node to other
            foreach (BuildNode other in nodes)
            {
                if (other == null) continue;
                
                for (int i = 0; i < other.anchors.Count; i++)
                {
                    BuildAnchor anchor = other.anchors.Values.ToList()[i];
                    int anchorId = other.anchors.Keys.ToList()[i];
                    
                    if (anchor.child.transform.position == node.transform.position && other.CheckPermissionToBuild(anchorId))
                    {
                        anchors.Add(new NodeAnchor
                        {
                            nodeId = other.nodeId,
                            anchorId = anchor.AnchorId,
                            childId = nodeData.id,
                        });
                    }
                }
            }

            Destroy(node.gameObject);
         
            buildingData.AddNode(nodeData.data, anchors);
            return nodeData;
        }
        
        public int GetRandomId()
        {
            int id;
            do
            {
                id = Random.Range(0, 100000);
            } while (buildingData.Value.nodes.Exists(x => x.id == id));
            
            return id;
        }
        
        #region Save

        public JSONObject Save()
        {
            throw new System.NotImplementedException();
        }

        public JSONObject GetDefaultSave()
        {
            throw new System.NotImplementedException();
        }

        public void Load(JSONObject json)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}