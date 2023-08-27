using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTemp : NetworkBehaviour
{
    public Camera playerCamera;
    public GameObject boatPrefab;
    private Boat boat;
    private bool buildMode = false;
    

    [SerializeField] private float rayLength = 7;
    [SerializeField] private Material previewMat;

    public BlockObject basePlatform;
    public List<BlockObject> blockObjects;
    private int currentBlockObjectIndex = 0;

    private GameObject structurePreview;
    private int rotation = 0;

    private void Start()
    {
        blockObjects.Add(basePlatform);
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
        playerCamera.transform.gameObject.SetActive(IsOwner);
    }
    
    private void Update()
    {
        ProcessInput();
        if (buildMode) BuildMode();
    }
    
    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            ActivatePreview(buildMode);
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject go = new GameObject();
            go.transform.position = transform.position + playerCamera.transform.forward * 2;
            boat = go.AddComponent<Boat>();
            boat.basePlatform = basePlatform;
            go.name = "Boat";
            // Rigidbody rb = go.AddComponent<Rigidbody>();
            // rb.useGravity = false;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentBlockObjectIndex += 1;
            currentBlockObjectIndex = Mathf.Clamp(currentBlockObjectIndex, 0, blockObjects.Count-1);
            UpdatePreview();
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentBlockObjectIndex -= 1;
            currentBlockObjectIndex = Mathf.Clamp(currentBlockObjectIndex, 0, blockObjects.Count-1);
            UpdatePreview();
        }

    }

    private void Rotate()
    {
        rotation += 1;
        if (rotation == 4) rotation = 0;
    }
    
    private void BuildMode()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayLength, Color.blue);

        structurePreview.transform.position = playerCamera.transform.position + playerCamera.transform.forward * rayLength;
        structurePreview.transform.rotation = Quaternion.Euler(new Vector3(0, 90 * rotation, 0));
        previewMat.SetColor("_BaseColor", new Color(255, 0, 0, 25));
        

        RaycastHit[] hits;
        hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, rayLength, LayerMask.GetMask("Block"));
        if (hits.Length != 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.TryGetComponent<Block>(out Block block))
                {
                    if (block.CheckIfCanSpawnBlock(blockObjects[currentBlockObjectIndex], rotation))
                    {
                        structurePreview.transform.position = block.transform.position;
                        previewMat.SetColor("_BaseColor", new Color(0, 255, 0, 25));
                    }
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (block.SpawnBlock(blockObjects[currentBlockObjectIndex], rotation)) return; //--> Prevent spawning multiple blocks at once
                    }
                    else if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (block.DestroyBlock()) return; //--> Prevent destroying multiple blocks at once
                    }
                }
            }
        }
    }
    

    private void ActivatePreview(bool activate)
    {
        if (activate)
        {
            if (structurePreview == null)
            {
                structurePreview = Instantiate(blockObjects[currentBlockObjectIndex].blockPrefab);
                List<Material> mats = new();
                Renderer renderer = structurePreview.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = previewMat; // TODO : fix this (apply it to multiple mats)
                structurePreview.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            Destroy(structurePreview);
        }
    }

    private void UpdatePreview()
    {
        Destroy(structurePreview);
        structurePreview = Instantiate(blockObjects[currentBlockObjectIndex].blockPrefab);
        List<Material> mats = new();
        Renderer renderer = structurePreview.GetComponent<MeshRenderer>();
        renderer.material = previewMat; // TODO : fix this (apply it to multiple mats)
    }

}
