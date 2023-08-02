using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    List<List<List<Block>>> blocks = new ();
    private GameObject boat;
    private float scale = 3;
    private Vector3Int negativeOffset = Vector3Int.one;
    
    public void InitGrid(GameObject _boat, BlockObject _basePlatform)
    {
        boat = _boat;
        
        //Populate list
        for (int x = 0; x < 3; x++)
        {
            blocks.Add(new List<List<Block>>());
            for (int y = 0; y < 3; y++)
            {
                blocks[x].Add(new List<Block>());
                for (int z = 0; z < 3; z++)
                {
                    blocks[x][y].Add(CreateBlockAtPos(new Vector3(x, y, z)));
                }
            }
        }
        blocks[1][1][1].ForceSpawnBlock(_basePlatform);
    }

    private Block CreateBlockAtPos(Vector3 pos)
    {
        GameObject go = Instantiate(Resources.Load("Block") as GameObject, boat.transform);
        go.transform.localPosition = pos * scale;
        Block block = go.GetComponent<Block>();
        block.grid = this;
        block.gridPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z) - negativeOffset;
        return block;
    }
    
    public Block GetBlockAtPos(Vector3Int pos)
    {
        if (pos.x + negativeOffset.x < 0 || pos.x + negativeOffset.x >= blocks.Count) return null;
        if (pos.y + negativeOffset.y < 0 || pos.y + negativeOffset.y >= blocks[pos.x + negativeOffset.x].Count) return null;
        if (pos.z + negativeOffset.z < 0 || pos.z + negativeOffset.z >= blocks[pos.x + negativeOffset.x][pos.y + negativeOffset.y].Count) return null;

        return blocks[pos.x + negativeOffset.x][pos.y + negativeOffset.y][pos.z + negativeOffset.z];
    }
}
