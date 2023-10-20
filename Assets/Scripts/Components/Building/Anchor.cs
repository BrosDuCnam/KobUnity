using System.Collections.Generic;
using UnityEngine;

namespace Components.Building
{ 
    public class Anchor : MonoBehaviour
    {
        [SerializeField] private List<Block> possibleBlocks = new ();
        [SerializeField] private Block parentBlock;
        [SerializeField] private Block childBlock;
        [SerializeField] private new Collider collider;
        
        public List<Block> PossibleBlocks => possibleBlocks;
        public Collider Collider => collider;
        
        public bool IsOccupied => childBlock != null;
        public bool IsAvailable => parentBlock.IsAnchorAvailable(this);
        
        public void SetParentBlock(Block block)
        {
            parentBlock = block;
        }
        
        public void SetChildBlock(Block block)
        {
            childBlock = block;
        }
    }
}