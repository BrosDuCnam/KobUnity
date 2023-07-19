using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject boatPrefab;
    private Boat boat;
    private bool buildMode = false;

    private float rayLength = 3;
    
    void Update()
    {
        ProcessInput();

        if (buildMode)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayLength, Color.blue);
            ProcessClick();
        }
    }
    
    void ProcessClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward * 3, out RaycastHit hit);
            if (hit.collider != null)
            {
                GameObject go = hit.collider.gameObject;
                Buildable buildable = go.GetComponent<Buildable>();
                if (buildable != null)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        boat.AddPlatform(buildable);
                    }
                    else if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        boat.RemovePlatform(buildable);
                    }
                }
            }
        }
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject go = Instantiate(boatPrefab);
            go.transform.position = transform.position + playerCamera.transform.forward * 2;
            boat = go.GetComponent<Boat>();
            boat.Spawn();
        }
    }
}
