using System;
using System.Collections;
using System.Collections.Generic;
using Network.Data;
using Scriptable;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private Grid grid = new Grid();
    public BlockObject basePlatform;
    private BoatData boatData;
    

    private void Start()
    {
        grid.InitGrid(basePlatform, this);
        boatData = GetComponent<BoatData>();
        
        // boatData.OnValueChanged.AddListener();
    }
    
    
}
