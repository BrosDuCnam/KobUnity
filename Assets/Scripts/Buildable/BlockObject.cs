using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public enum SpawnType
{
    OnEdge,
    OnGround
}

[CreateAssetMenu(fileName = "BlockObject", menuName = "ScriptableObjects/BlockObject", order = 1)]
public class BlockObject : ScriptableObject
{
    public GameObject blockPrefab;
    public bool onGrid = false;
    public SpawnType spawnType = SpawnType.OnEdge;
    public List<Vector3Int> occupiedSlots = new ();
}
