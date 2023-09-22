using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Components.Data
{
    public struct Boat
    {
        private Dictionary<Vector3Int, Block> grid;

        public List<List<List<Block>>> GetBlockList()
        {
            List<List<List<Block>>> blocks = new List<List<List<Block>>>();
            foreach (var block in grid)
            {
                if (blocks[block.Key.y] == null)
                {
                    blocks[block.Key.y] = new List<List<Block>>();
                }
                
                if (blocks[block.Key.y][block.Key.z] == null)
                {
                    blocks[block.Key.y][block.Key.z] = new List<Block>();
                }
                
                blocks[block.Key.y][block.Key.z][block.Key.x] = block.Value;
            }
            
            return blocks;
        }
        
        public void SetBlock(Vector3Int position, Block block)
        {
            if (grid.ContainsKey(position))
            {
                grid[position] = block;
            }
            else
            {
                grid.Add(position, block);
            }
        }
        
        public void RemoveBlock(Vector3Int position)
        {
            grid.Remove(position);
        }
        
        
        // le reste
    }
}