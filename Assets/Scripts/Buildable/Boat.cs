using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private Grid grid = new Grid();
    public BlockObject basePlatform;
    

    private void Start()
    {
        grid.InitGrid(basePlatform, this);
    }
}
