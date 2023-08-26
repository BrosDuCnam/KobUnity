using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Lumin;

public enum FloaterType
{
    Single,
    Double,
    Triple,
    Full
}

public class Block : MonoBehaviour
{
    [HideInInspector] public Grid grid;
    public Vector3Int gridPos; // Pos (location) regarding the origin of the boat :: goes negative
    private Collider blockCollider;
    [SerializeField] private BlockObject blockObject;
    [SerializeField] private GameObject blockObjectInstance;
    private int rotation;

    public GameObject currentfloater = null;
    private FloaterType currentFloaterType;
    
    public BlockObject holderObject = null;
    
    
    private void Start()
    {
        blockCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Spawn given block object. Returns true if the block has been successfully spawned and false otherwise
    /// </summary>
    /// <param name="_blockObject"></param>
    /// <returns></returns>
    public bool SpawnBlock(BlockObject _blockObject, int _rotation)
    {
        if (!CheckIfCanSpawnBlock(_blockObject, rotation)) return false;
        
        blockObject = _blockObject;
        GameObject go = Instantiate(blockObject.blockPrefab, transform);
        go.transform.parent = transform;
        rotation = _rotation;
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 90 * _rotation, 0));

        AttachBoatToFurniture(go);

        blockObjectInstance = go;
        grid.ExtendGrid(gridPos);

        PopulateNeighbours(_blockObject, _rotation);
        
        UpdateFloaterAndNeighbours();

        return true;
    }

    private void AttachBoatToFurniture(GameObject go)
    {
        foreach (Furniture furniture in go.GetComponents<Furniture>())
        {
            furniture.AttachBoat(grid.boat);
        }
    }

    private bool IsNeighbourFree(BlockObject _blockObject, int _rotation)
    {
        if (_blockObject.occupiedSlots.Count == 0) return true;
        
        foreach (Vector3Int occupiedSlot in _blockObject.occupiedSlots)
        {
            Vector3Int rotatedVector = RotateVector(occupiedSlot, _rotation);
            Block neighbour = grid.GetBlockAtPos(gridPos + rotatedVector);
            if (neighbour == null) break;
            if (neighbour.holderObject != null || neighbour.blockObject != null) return false;
        }
        return true;
    }

    private void PopulateNeighbours(BlockObject _blockObject, int _rotation)
    {
        if (_blockObject.occupiedSlots.Count == 0) return;

        foreach (Vector3Int occupiedSlot in _blockObject.occupiedSlots)
        {
            Vector3Int rotatedVector = RotateVector(occupiedSlot, _rotation);
            grid.GetBlockAtPos(gridPos + rotatedVector).holderObject = _blockObject;
        }
    }

    private void ClearOccupiedSlots(BlockObject _blockObject, int _rotation)
    {
        if (_blockObject.occupiedSlots.Count == 0) return;
        
        foreach (Vector3Int occupiedSlot in _blockObject.occupiedSlots)
        {
            Vector3Int rotatedVector = RotateVector(occupiedSlot, _rotation);
            grid.GetBlockAtPos(gridPos + rotatedVector).holderObject = null;
        }
    }
    
    public bool CheckIfCanSpawnBlock(BlockObject _blockObject, int _rotation)
    {
        if (holderObject != null) return false;
        if (!IsNeighbourFree(_blockObject, _rotation)) return false;
        
        rotation = _rotation;
        if (blockObject != null) return false; // Do not spawn block if already spawned
        switch (_blockObject.spawnType)
        {
            case SpawnType.OnEdgePlatform:
                if (!HasEdgePlatformBlocks()) return false;
                break;
            case SpawnType.OnGround:
                if (!HasGroundBlocks()) return false;
                break;
            case SpawnType.OnPlatformFront:
                if (!HasFrontPlatform()) return false;
                break;
        }

        return true;
    }

    private bool HasEdgePlatformBlocks()
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
            if (block.blockObject == null) continue;
            if (block.blockObject.blockType == BlockType.BasePlatform) return true;
        }
        
        return false;
    }
    private bool HasGroundBlocks()
    {
        //TODO: Check if there is a block below this one
        return true;
    }

    private bool HasFrontPlatform()
    {
        Vector3Int slotToCheck = RotateVector(new Vector3Int(0, 0, 1), rotation); // Front
        Block block = grid.GetBlockAtPos(slotToCheck + gridPos);
        if (block == null) return false;
        if (block.blockObject == null) return false;
        if (block.blockObject.blockType == BlockType.BasePlatform) return true;
        return false;
    }

    /// <summary>
    /// Destroy given block object. Returns true if the block has been successfully destroyed and false otherwise
    /// </summary>
    /// <returns></returns>
    public bool DestroyBlock()
    {
        if (blockObject != null && gridPos != Vector3Int.zero)
        {
            ClearOccupiedSlots(blockObject, rotation);
            Destroy(blockObjectInstance);
            blockObject = null;
            UpdateFloaterAndNeighbours();
            return true;
        }
        return false;
    }
    
    //Used only to spawn the first planks when a boat is created 
    public void ForceSpawnBlock(BlockObject _blockObject)
    {
        blockObject = _blockObject;
        
        GameObject go = Instantiate(blockObject.blockPrefab, transform);
        go.transform.parent = transform;
    }

    private Vector3Int RotateVector(Vector3Int vec, int _rotation)
    {
        for (int i = 0; i < _rotation; i++)
        {
            vec = new Vector3Int(vec.z, vec.y, -vec.x);
        }

        return vec;
    }

    #region Floaters
    
    private void UpdateFloaterAndNeighbours()
    {
        UpdateFloaterState();
    }

    private void UpdateFloaterState()
    {
        if (blockObject != null) return;

        List<Vector3Int> validNeighbours = GetBasePlatformNeighbours();

        if (currentfloater != null) Destroy(currentfloater);
        
        switch (validNeighbours.Count)
        {
            case 0:
                return;
            case 1:
                currentfloater = Instantiate(Resources.Load("SingleFloater") as GameObject, transform);
                break;
            case 2:
                currentfloater = Instantiate(Resources.Load("DoubleFloater") as GameObject, transform);
                break;
            case 3:
                currentfloater = Instantiate(Resources.Load("TripleFloater") as GameObject, transform);
                break;
            case 4:
                currentfloater = Instantiate(Resources.Load("FullFloater") as GameObject, transform);
                break;
        }
        currentfloater.transform.rotation = Quaternion.Euler(0, 90 * GetRotationForFloater(validNeighbours), 0);
    }

    /// <summary>
    /// Get neighbours that are of type "BasePlatform" 
    /// </summary>
    /// <returns>List of neighbours of type "BasePlatform" </returns>
    private List<Vector3Int> GetBasePlatformNeighbours()
    {
        List<Vector3Int> slotsToCheck = new List<Vector3Int>();
        
        slotsToCheck.Add(new Vector3Int(1, 0, 0));
        slotsToCheck.Add(new Vector3Int(-1, 0, 0));
        slotsToCheck.Add(new Vector3Int(0, 0, -1));
        slotsToCheck.Add(new Vector3Int(0, 0, 1));

        int negOffset = 0;
        
        //Remove neighbours that are not a base platform
        for (int i = 0; i < 4; i++)
        {
            Block neighbourBlock = grid.GetBlockAtPos(slotsToCheck[i - negOffset] + gridPos);
            if (neighbourBlock.blockObject == null)
            {
                slotsToCheck.RemoveAt(i - negOffset);
                negOffset += 1;
            }
            else if (neighbourBlock.blockObject.blockType != BlockType.BasePlatform)
            {
                slotsToCheck.RemoveAt(i - negOffset);
                negOffset += 1;
            }
        }
        return slotsToCheck;
    }

    public void ActivateFloater(FloaterType _type, int _rotation)
    {
        
    }

    public void HideFloater()
    {
        
    }

    private int GetRotationForFloater(List<Vector3Int> walls)
    {
        switch (walls.Count)
        {
            case 1:
                return ConvertCoordsToAngle(walls[0]);
            case 2:
                return 0;
            case 3:
                int sum = 0;
                foreach (Vector3Int wall in walls) sum += ConvertCoordsToAngle(wall);
                print(sum);
                if (sum == 3) return 0;
                if (sum == 4) return 1;
                if (sum == 5) return 2;
                if (sum == 6) return 3;
                break;
        }
        return 0;
    }

    private int ConvertCoordsToAngle(Vector3Int pos)
    {
        if (pos == new Vector3Int(0, 0, 1)) return 0;
        if (pos == new Vector3Int(1, 0, 0)) return 1;
        if (pos == new Vector3Int(0, 0, -1)) return 2;
        if (pos == new Vector3Int(-1, 0, 0)) return 3;
        return 0;
    }

    #endregion
    
    
}