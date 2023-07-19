using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] private GameObject basePlatform;
    [SerializeField] private int scale = 3;
    private List<List<Buildable>> grid = new ();
    private int negativeX = 0;
    private int negativeY = 0;
    
    [System.Serializable]
    public struct GridPos
    {
        public int x;
        public int y;
    }
    
    public void Spawn()
    {
        grid.Add(new List<Buildable>());
        GameObject platform = Instantiate(basePlatform, transform);
        Buildable buildable = platform.GetComponent<Buildable>();
        buildable.meshRenderer.enabled = true;
        buildable.collider.enabled = true;
        buildable.collider.isTrigger = false;
        platform.transform.position = transform.position;
        buildable.SetGridPos(0, 0);
        grid[0].Add(buildable);
        GridPos pos = new GridPos();
        pos.x = 0;
        pos.y = 0;
        ExtendGrid(pos);
        AddEdges(pos);
    }
    
    /// <summary>
    /// Use this function to add a platform to the boat
    /// </summary>
    /// <param name="platform"></param>
    public void AddPlatform(Buildable platform)
    {
        GridPos pos = platform.gridPos;
        platform.collider.isTrigger = false;
        platform.meshRenderer.enabled = true;
        ExtendGrid(pos);
        AddEdges(pos);
    }

    // Handle the activation of the trigger where a platform can be built
    private void AddEdges(GridPos _pos)
    {
        grid[_pos.x+1 + negativeX][_pos.y + negativeY].collider.enabled = true;
        grid[_pos.x-1 + negativeX][_pos.y + negativeY].collider.enabled = true;
        grid[_pos.x + negativeX][_pos.y+1 + negativeY].collider.enabled = true;
        grid[_pos.x + negativeX][_pos.y-1 + negativeY].collider.enabled = true;
        
        grid[_pos.x+1 + negativeX][_pos.y + negativeY].edgeObject.SetActive(true);
        grid[_pos.x-1 + negativeX][_pos.y + negativeY].edgeObject.SetActive(true);
        grid[_pos.x + negativeX][_pos.y+1 + negativeY].edgeObject.SetActive(true);
        grid[_pos.x + negativeX][_pos.y-1 + negativeY].edgeObject.SetActive(true);
        
    }
    
    //Extend the grid if the platform is at the edge
    private void ExtendGrid(GridPos _pos)
    {
        if (_pos.x + negativeX + 1 == grid.Count)
        {
            List<Buildable> list = new List<Buildable>();
            for (int i = 0; i < grid[0].Count; i++)
            {
                GameObject platform = Instantiate(basePlatform, transform);
                platform.transform.position = transform.position + new Vector3(_pos.x + 1, 0, i - negativeY) * scale;
                Buildable buildable = platform.GetComponent<Buildable>();
                buildable.SetGridPos(_pos.x + 1, i - negativeY);
                // print("pos at : " + (_pos.x + 1 ) + " " + (i - negativeY) + " positive");
                list.Add(buildable);
            }
            grid.Add(list);
        }
        if (_pos.y + negativeY + 1 == grid[0].Count)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                GameObject platform = Instantiate(basePlatform, transform);
                platform.transform.position = transform.position + new Vector3(i - negativeX, 0, _pos.y + 1) * scale;
                Buildable buildable = platform.GetComponent<Buildable>();
                buildable.SetGridPos(i - negativeX, _pos.y + 1);
                // print("pos at : " + (i - negativeX) + " " + (_pos.y + 1) + " positive");
                grid[i].Add(buildable);
            }
        }
        if (_pos.x + negativeX - 1 < 0)
        {
            List<Buildable> list = new List<Buildable>();
            for (int i = 0; i < grid[0].Count; i++)
            {
                GameObject platform = Instantiate(basePlatform, transform);
                platform.transform.position = transform.position + new Vector3(_pos.x - 1, 0, i - negativeY) * scale;
                Buildable buildable = platform.GetComponent<Buildable>();
                buildable.SetGridPos(_pos.x - 1, i - negativeY);
                // print("pos at : " + (_pos.x - 1) + " " + (i - negativeY) + " negative");
                list.Add(buildable);
            }
            grid.Insert(0, list);
            negativeX++;
        }
        if (_pos.y + negativeY - 1 < 0)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                GameObject platform = Instantiate(basePlatform, transform);
                platform.transform.position = transform.position + new Vector3(i - negativeX, 0, _pos.y - 1) * scale;
                Buildable buildable = platform.GetComponent<Buildable>();
                buildable.SetGridPos(i - negativeX, _pos.y - 1);
                // print("pos at : " + (i - negativeX) + " " + (_pos.y - 1) + " negative");
                grid[i].Insert(0, buildable);
            }
            negativeY++;
        }
    }
}
