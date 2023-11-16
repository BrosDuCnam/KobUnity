using System;
using System.Collections.Generic;
using Components.Building.AnchorConditions;
using Scriptable;
using UnityEngine;
using UnityEngine.Events;

namespace Components.Building
{
    public class Anchor : MonoBehaviour
    {
        [SerializeField] private List<AnchorCondition> conditions = new();
        [SerializeField] private List<Block> possibleBlocks = new();
        [SerializeField] public Block parentBlock;
        [SerializeField] public Block childBlock;

        public List<Block> PossibleBlocks => possibleBlocks;

        public bool IsOccupied => childBlock != null;
        public bool IsAvailable => parentBlock.IsAnchorAvailable(this) && conditions.TrueForAll(c => c.IsSatisfied(this));

        public void SetParentBlock(Block block)
        {
            parentBlock = block;
        }

        public void SetChildBlock(Block block)
        {
            childBlock = block;
        }
        
        private void Start()
        {
            conditions.AddRange(GetComponents<AnchorCondition>());
        }
        
        public string GetSelfPath()
        {
            string path = transform.GetSiblingIndex().ToString();
            Transform parent = transform.parent;
            while (parent != parentBlock.transform && parent.transform.name != "Anchors")
            {
                path = parent.name + "_" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}