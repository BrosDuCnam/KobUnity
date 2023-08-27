using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public enum SpawnType
{
    OnEdgePlatform,
    OnPlatformFront,
    OnGround
}

public enum BlockType
{
    BasePlatform,
    Holder,
    Other, 
    Platform
}

[CreateAssetMenu(fileName = "BlockObject", menuName = "ScriptableObjects/BlockObject", order = 1)]
public class BlockObject : ScriptableObject
{
    public GameObject blockPrefab;
    public bool onGrid = false;
    public SpawnType spawnType = SpawnType.OnEdgePlatform;
    public BlockType blockType = BlockType.Other;
    public List<Vector3Int> occupiedSlots = new();
}
