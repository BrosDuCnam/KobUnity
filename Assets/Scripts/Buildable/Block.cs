using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [HideInInspector] public Grid grid;
    public Vector3Int gridPos;
    private Collider blockCollider;
    [SerializeField] private BlockObject blockObject;
    
    private void Start()
    {
        blockCollider = GetComponent<Collider>();
    }

    public bool SpawnBlock(BlockObject _blockObject)
    {
        if (!CheckIfCanSpawnBlock(_blockObject)) return false;
        
        blockObject = _blockObject;
        GameObject go = Instantiate(blockObject.blockPrefab, transform);
        go.transform.parent = transform;

        return true;
    }
    
    private bool CheckIfCanSpawnBlock(BlockObject _blockObject)
    {
        switch (_blockObject.spawnType)
        {
            case SpawnType.OnEdge:
                if (!HasEdgeBlocks()) return false;
                break;
            case SpawnType.OnGround:
                if (!HasGroundBlocks()) return false;
                break;
        }

        return true;
    }

    private bool HasEdgeBlocks()
    {
        
        List<Vector3Int> slotsToCheck = new List<Vector3Int>();
        
        slotsToCheck.Add(new Vector3Int(1, 0, 0));
        slotsToCheck.Add(new Vector3Int(-1, 0, 0));
        slotsToCheck.Add(new Vector3Int(0, 0, -1));
        slotsToCheck.Add(new Vector3Int(0, 0, 1));
        
        foreach (Vector3Int slot in slotsToCheck)
        {
            Block block = grid.GetBlockAtPos(slot + gridPos);
            if (block == null) continue;
            if (grid.GetBlockAtPos(slot + gridPos).blockObject != null) return true;
        }
        
        return false;
    }
    private bool HasGroundBlocks()
    {
        //TODO: Check if there is a block below this one
        return true;
    }
    
    public void ForceSpawnBlock(BlockObject _blockObject)
    {
        blockObject = _blockObject;
        
        GameObject go = Instantiate(blockObject.blockPrefab, transform);
        go.transform.parent = transform;
    }
}
