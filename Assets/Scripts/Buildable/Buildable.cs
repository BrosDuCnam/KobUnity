using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{

    /*[HideInInspector]*/ public Boat.GridPos gridPos;
    public Collider collider;
    public MeshRenderer meshRenderer;
    public bool onGrid;
    public GameObject edgeObject;
    
    public void SetGridPos(int x, int y)
    {
        gridPos.x = x;
        gridPos.y = y;
    }
    
    

}
