using System;
using System.Collections.Generic;
using Components.Data;
using Network.Data;
using UnityEngine;
using Utils;

namespace Components.Building
{
    public class BuildController : MonoBehaviour
    {
        public GraphNodeBuild build = new GraphNodeBuild();
        
        [Header("References")]
        [SerializeField] private NetworkBuild networkBuild;
        [SerializeField] private List<Block> blocks = new List<Block>();

        private void Awake()
        {
            networkBuild.OnValueChanged.AddListener(OnNetworkBuildChanged);
        }

        private void OnNetworkBuildChanged(GraphNodeBuild graph)
        {
            List<BlockNode> changes = new List<BlockNode>();
            foreach (var node in graph.nodes)
            {
                if (build.nodes.Contains(node)) continue;
                changes.Add(node);
            }
            
            RefreshBlocks(changes);
        }
        
        public Block GetBlock(int id)
        {
            return blocks.Find(x => x.id == id);
        }
        
        public void RefreshBlocks(List<BlockNode> blocks)
        {
            foreach (BlockNode node in blocks)
            {
                Block block = GetBlock(node.ID);
                if (block == null)
                {
                    block = Instantiate(UResources.GetBlockByType(node.type), transform);
                    block.id = node.ID;
                    block.buildController = this;
                }
                
                List<(int anchor, int blockId)> parents = BlockNode.GetAnchorData(node.parents);
                for (int i = 0; i < parents.Count; i++)
                {
                    Block parent = GetBlock(parents[i].blockId);
                    Anchor anchor = parent.childrenAnchor[parents[i].anchor.ToString()];

                    if (i == 0)
                    {
                        block.TryToPlaceOn(anchor, false);
                    }
                    else
                    {
                        anchor.childBlock = block;
                    }
                }
            }
        }
    }
}