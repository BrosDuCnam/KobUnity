using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_Buildable : MonoBehaviour
{

    [HideInInspector]public Old_Boat.GridPos gridPos;
    public Collider collider;
    public MeshRenderer meshRenderer;
    public bool onGrid;
    public GameObject edgeObject;
    private Old_Boat boat;
    
    [HideInInspector] public bool isPlatform = false;

    public void SetParentBoat(Old_Boat parentBoat)
    {
        boat = parentBoat;
    }
    
    public Old_Boat GetParentBoat()
    {
        return boat;
    }
    
    public void SetGridPos(int x, int y)
    {
        gridPos.x = x;
        gridPos.y = y;
    }
    
    public void SetAsEdge()
    {
        collider.enabled = true;
        edgeObject.SetActive(true);
    }
    
    public void RemoveEdge()
    {
        collider.enabled = false;
        edgeObject.SetActive(false);
    }
    
    public void SetAsPlatform()
    {
        collider.isTrigger = false;
        meshRenderer.enabled = true;
        isPlatform = true;
    }
    
    public void RemovePlatform()
    {
        collider.isTrigger = true;
        meshRenderer.enabled = false;
        isPlatform = false;
    }


}
