using System.Collections.Generic;
using System.Linq;
using Data.Building;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Network.Data
{
    public class BuildingData : NetworkData<Build>
    {
        public UnityEvent<List<BuildChange>> OnBuildChanged = new ();
        
        private NetworkList<NodeData> _nodes = null;
        private NetworkList<NodeAnchor> _anchors = null;

        public override Build Value
        {
            protected set
            {
                Build oldValue = this.value;
                
                if (this.value.Equals(value)) return;
                base.Value = value;
                
                OnBuildChanged.Invoke(Build.Compare(oldValue, value));
            }
        }
        
        private void Awake()
        {
            _nodes = new NetworkList<NodeData>();
            _anchors = new NetworkList<NodeAnchor>();
            
            value = new Build() { nodes = new List<Node>() }; // Initialize empty build
        }

        public override void OnNetworkSpawn()
        {
            _nodes.OnListChanged += OnServerNodeValueChanged;
            _anchors.OnListChanged += OnServerAnchorValueChanged;

            base.OnNetworkSpawn();
        }

        #region Events

        private void OnServerAnchorValueChanged(NetworkListEvent<NodeAnchor> change)
        {
            return;
            
            if (change.Index < 0 || change.Index >= _anchors.Count) return;

            Node node = GetNode(change.Value.nodeId, true);
            for (int i = 0; i < node.anchors.Count; i++)
            {
                if (node.anchors[i].anchorId != change.Value.anchorId) continue;

                node.anchors[i] = change.Value;
                break;
            }
            node.id = change.Value.nodeId;
            node.data.nodeId = change.Value.nodeId;

            Build build = GetValue();
            if (build.nodes.Any(n => n.id == node.id))
            {
                build.nodes[build.nodes.FindIndex(n => n.id == node.id)] = node;
            }
            else
            {
                build.nodes.Add(node);
            }

            Value = build;
        }

        private void OnServerNodeValueChanged(NetworkListEvent<NodeData> change)
        {
            if (change.Index < 0 || change.Index >= _nodes.Count) return;

            Node node = GetNode(change.Value.nodeId, true);
            node.data = change.Value;

            Build build = GetValue();
            if (build.nodes.Any(n => n.id == node.id))
            {
                build.nodes[build.nodes.FindIndex(n => n.id == node.id)] = node;
            }
            else
            {
                build.nodes.Add(node);
            }

            Value = build;
        }


        #endregion

        // Raw = true means that the node will be populated with anchors and data
        public Node GetNode(int id, bool raw = false)
        {
            if (!raw) return GetValue().nodes.Find(n => n.id == id);

            Node node = new Node()
            {
                id = id,
                anchors = new List<NodeAnchor>(),
                data = new NodeData()
            };

            foreach (var anchor in _anchors)
            {
                if (anchor.nodeId == node.id)
                {
                    node.anchors.Add(anchor);
                }
            }

            foreach (var data in _nodes)
            {
                if (data.nodeId == node.id)
                {
                    node.data = data;
                }
            }

            return node;

        }

        public override Build GetValue()
        {
            var build = new Build();

            Dictionary<int, Node> nodeMap = new Dictionary<int, Node>(); // id -> node

            foreach (var anchor in _anchors)
            {
                if (nodeMap.ContainsKey(anchor.nodeId))
                {
                    nodeMap[anchor.nodeId].anchors.Add(anchor);
                }
                else
                {
                    nodeMap.Add(anchor.nodeId, new Node()
                    {
                        id = anchor.nodeId,
                        anchors = new List<NodeAnchor>() { anchor }
                    });
                }
            }

            foreach (var data in _nodes)
            {
                if (nodeMap.ContainsKey(data.nodeId))
                {
                    Node node = nodeMap[data.nodeId];
                    node.data = data;

                    nodeMap[data.nodeId] = node;
                }
                else
                {
                    nodeMap.Add(data.nodeId, new Node()
                    {
                        id = data.nodeId,
                        data = data,
                        anchors = new List<NodeAnchor>()
                    });
                }
            }

            build.nodes = nodeMap.Values.ToList();
            return build;
        }

        #region AddNode

        public void AddNode(NodeData data, List<NodeAnchor> anchors)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _AddNode(data, anchors);
            }
            else
            {
                AddNodeServerRpc(data, anchors.ToArray());
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddNodeServerRpc(NodeData data, NodeAnchor[] anchors)
        {
            _AddNode(data, anchors.ToList());
        }
        
        private void _AddNode(NodeData data, List<NodeAnchor> anchors)
        {
            foreach (var anchor in anchors)
            {
                _anchors.Add(anchor);
            }
            _nodes.Add(data);
        }

        #endregion

        #region RemoveNode
        
        public void RemoveNode(int id)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _RemoveNode(id);
            }
            else
            {
                RemoveNodeServerRpc(id);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemoveNodeServerRpc(int id)
        {
            _RemoveNode(id);
        }
        
        private void _RemoveNode(int id)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                if (_nodes[i].nodeId == id)
                {
                    _nodes.RemoveAt(i);
                    break;
                }
            }
            
            for (int i = _anchors.Count - 1; i >= 0; i--)
            {
                if (_anchors[i].nodeId == id)
                {
                    _anchors.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}