using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    List<List<List<Block>>> blocks = new ();
    public Boat boat;
    private float scale = 3;
    private Vector3Int negativeOffset = Vector3Int.one; // Always > 0

    public void Refresh(Components.Data.Boat boatData)
    {
        foreach (var block in boatData.GetBlockList())
        {
            
        }
    }

    public void InitGrid(BlockObject _basePlatform, Boat _boat)
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
                    blocks[x][y].Add(CreateBlockAtListPos(new Vector3(x, y, z)));
                }
            }
        }
        blocks[1][1][1].ForceSpawnBlock(_basePlatform); // Create boat original block
    }

    /// <summary>
    /// Creates a platform at the given list position 
    /// </summary>
    /// <param name="pos">List position /!\ Must be positive</param>
    /// <returns></returns>
    private Block CreateBlockAtListPos(Vector3 pos)
    {
        GameObject go = Instantiate(Resources.Load("Block") as GameObject, boat.transform);
        go.transform.localPosition = (pos - negativeOffset) * scale;
        Block block = go.GetComponent<Block>();
        block.grid = this;
        block.gridPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z) - negativeOffset;
        return block;
    }
    
    /// <summary>
    /// Return block at absolute pos
    /// </summary>
    /// <param name="pos">Block absolute position (goes negative)</param>
    /// <returns></returns>
    public Block GetBlockAtPos(Vector3Int pos)
    {
        if (pos.x + negativeOffset.x < 0 || pos.x + negativeOffset.x >= blocks.Count) return null;
        if (pos.y + negativeOffset.y < 0 || pos.y + negativeOffset.y >= blocks[pos.x + negativeOffset.x].Count) return null;
        if (pos.z + negativeOffset.z < 0 || pos.z + negativeOffset.z >= blocks[pos.x + negativeOffset.x][pos.y + negativeOffset.y].Count) return null;

        return blocks[pos.x + negativeOffset.x][pos.y + negativeOffset.y][pos.z + negativeOffset.z];
    }

    public void ExtendGrid(Vector3Int pos)
    {
        Vector3Int posInGrid = pos + negativeOffset;

        if (posInGrid.x + 1 == blocks.Count)
        {
            blocks.Add(CreateYList(posInGrid, true));
        }
        else if (posInGrid.x - 1 < 0)
        {
            blocks.Insert(0, CreateYList(posInGrid, false));
            negativeOffset.x++;
        }
        if (posInGrid.y + 1 == blocks[0].Count) // Vertical positive implementation (seems to work but tested very quickly) TODO : Vertical negative implementation
        {
            for (int x = 0; x < blocks.Count; x++)
            {
                List<Block> zList = new();

                for (int z = 0; z < blocks[0][0].Count; z++)
                {
                    zList.Add(CreateBlockAtListPos(new Vector3(x, posInGrid.y + 1, z)));
                }
                blocks[x].Add(zList);
            }
        }
        if (posInGrid.z + 1 == blocks[0][0].Count)
        {
            for (int x = 0; x < blocks.Count; x++)
            {
                for (int y = 0; y < blocks[0].Count; y++)
                {
                    blocks[x][y].Add(CreateBlockAtListPos(new Vector3(x, y, posInGrid.z + 1)));
                }
            }
        }
        else if (posInGrid.z -1 < 0)
        {
            for (int x = 0; x < blocks.Count; x++)
            {
                for (int y = 0; y < blocks[0].Count; y++)
                {
                    blocks[x][y].Insert(0, CreateBlockAtListPos(new Vector3(x, y, posInGrid.z - 1)));
                }
            }
            negativeOffset.z++;
        }
    }

    private List<List<Block>> CreateYList(Vector3 _posInGrid, bool _additive)
    {
        List<List<Block>> yList = new();
        for (int y = 0; y < blocks[0].Count; y++)
        {
            yList.Add(new List<Block>());
            for (int z = 0; z < blocks[0][0].Count; z++)
            {
                float xPos = _additive ? _posInGrid.x + 1 : _posInGrid.x - 1;
                yList[y].Add(CreateBlockAtListPos(new Vector3(xPos, y, z)));
            }
        }

        return yList;
    }
}
