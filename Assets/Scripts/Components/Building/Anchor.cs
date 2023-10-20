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
        [SerializeField] private Block parentBlock;
        [SerializeField] private Block childBlock;

        public List<Block> PossibleBlocks => possibleBlocks;

        public Block ParentBlock => parentBlock;
        public Block ChildBlock => childBlock;

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
    }
}