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
            ApplyChangeList(changes);
        }
        
        private void ApplyChangeList(List<BuildChange> changes)
        {
            changes = changes.OrderBy(x => x.added).ToList(); // Removed first, added last
            
            List<BuildChange> failedChanges = new List<BuildChange>(changes);
            int rotationCount = 0;
            int totalChanges = changes.Count;

            while (rotationCount < totalChanges)
            {
                List<BuildChange> previousChanges = new List<BuildChange>(failedChanges);
                failedChanges.Clear();
                bool hasChanged = false;
                totalChanges = previousChanges.Count;

                foreach (var change in previousChanges)
                {
                    if (!ApplyChange(change))
                    {
                        failedChanges.Add(change);
                    }
                    else
                    {
                        hasChanged = true;
                        rotationCount = 0; // Réinitialiser le compteur car une modification a réussi
                    }
                }

                if (failedChanges.Count > 0 && !hasChanged)
                {
                    // Décaler la première modification à la fin de la liste
                    var firstChange = failedChanges[0];
                    failedChanges.RemoveAt(0);
                    failedChanges.Add(firstChange);
                    rotationCount++; // Incrémenter le compteur de rotation
                }
                else if (hasChanged)
                {
                    rotationCount = 0; // Réinitialiser si des modifications ont réussi
                }

                if ((!hasChanged || previousChanges.Count == 0) && rotationCount >= totalChanges)
                {
                    break; // Arrêter la boucle si une rotation complète est effectuée sans succès ou s'il n'y a plus de modifications
                }
            }
        }

        
        private bool ApplyChange(BuildChange change)
        {
            if (change.added)
            {
                if (change.data.isRoot && nodes.Count == 0)
                {
                    if (nodes.Count == 0)
                    {
                        // First node
                        BuildNode.Instantiate(BuildNode.BuildType.Platform, transform, Vector3.zero, this, change.data.nodeId);
                        return true;   
                    }
                    
                    return false;
                }
                
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
                                
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            else
            {
                // Find build node
                foreach (BuildNode buildNode in nodes)
                {
                    if (buildNode.nodeId != change.data.nodeId) continue;
                    
                    buildNode.Destroy();
                    return true;
                }
            }
            
            return false;
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
                // if is server
                if (buildingData.IsServer)
                {
                    // Add first node
                    buildingData.AddNode(new NodeData()
                    {
                        nodeId = GetRandomId(),
                        isRoot = true,
                        type = BuildNode.BuildType.Platform
                    }, new List<NodeAnchor>());
                }

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
            return buildingData.Value.Save();
        }

        public JSONObject GetDefaultSave()
        {
            throw new System.NotImplementedException();
        }

        public void Load(JSONObject json)
        {
            Build build = new Build();
            build.Load(json);
            
            buildingData.SetBuild(build);
        }
        
        #endregion
    }
}