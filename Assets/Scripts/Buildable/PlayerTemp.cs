using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject boatPrefab;
    private Boat boat;
    private bool buildMode = false;

    private float rayLength = 5;

    public BlockObject basePlatform;
    
    void Update()
    {
        ProcessInput();
        if (buildMode) ProcessClick();
    }
    
    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject go = new GameObject();
            go.transform.position = transform.position + Vector3.down * 2.5f + playerCamera.transform.forward * 2;
            boat = go.AddComponent<Boat>();
            boat.basePlatform = basePlatform;
            go.name = "Boat";
        }
    }
    
    void ProcessClick()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayLength, Color.blue);

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 3, LayerMask.GetMask("Block"));
            if (hits.Length != 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.TryGetComponent<Block>(out Block block))
                    {
                        // if (block.SpawnBlock(basePlatform)) return; --> Use this in the future to prevent spawning multiple blocks at once
                        block.SpawnBlock(basePlatform);

                    }
                }
            }
        }
    }
    
    // !! OLD !!
    // public Camera playerCamera;
    // public GameObject boatPrefab;
    // private Old_Boat boat;
    // private bool buildMode = false;
    //
    // private float rayLength = 3;
    //
    // void Update()
    // {
    //     ProcessInput();
    //
    //     if (buildMode)
    //     {
    //         Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayLength, Color.blue);
    //         ProcessClick();
    //     }
    // }
    //
    // void ProcessClick()
    // {
    //     if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
    //     {
    //         Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward * 3, out RaycastHit hit);
    //         if (hit.collider != null)
    //         {
    //             GameObject go = hit.collider.gameObject;
    //             Old_Buildable Old_Buildable = go.GetComponent<Old_Buildable>();
    //             if (Old_Buildable != null)
    //             {
    //                 boat = Old_Buildable.GetParentBoat();
    //                 
    //                 if (Input.GetKeyDown(KeyCode.Mouse0))
    //                 {
    //                     boat.AddPlatform(Old_Buildable);
    //                 }
    //                 else if (Input.GetKeyDown(KeyCode.Mouse1))
    //                 {
    //                     boat.RemovePlatform(Old_Buildable);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // void ProcessInput()
    // {
    //     if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         buildMode = !buildMode;
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         GameObject go = Instantiate(boatPrefab);
    //         go.transform.position = transform.position + playerCamera.transform.forward * 2;
    //         boat = go.GetComponent<Old_Boat>();
    //         boat.Spawn();
    //     }
    // }
}
