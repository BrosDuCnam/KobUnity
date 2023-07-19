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
        buildable.collider.enabled = true;
        buildable.SetAsPlatform();
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
        platform.SetAsPlatform();
        ExtendGrid(pos);
        AddEdges(pos);
    }
    
    /// <summary>
    /// Use this function to remove a platform from the boat
    /// </summary>
    /// <param name="platform"></param>
    public void RemovePlatform(Buildable platform)
    {
        platform.RemovePlatform();
        RecalculateEdges(platform.gridPos);
    }

    // Remove edges that don't have any platform around them
    private void RecalculateEdges(GridPos _pos)
    {
        foreach (GridPos edgePos in GetEdgesPos(_pos))
        {
            bool isPlatform = false;
            foreach (GridPos frac in GetEdgesPos(edgePos))
            {
                // Check if the platform is at the edge of the boat (prevent out of range exception)
                int x = frac.x + negativeX;
                int y = frac.y + negativeY;
                if (x >= grid.Count || x < 0) break;
                if (y >= grid[0].Count || y < 0) break;
                
                if (grid[x][y].isPlatform)
                {
                    isPlatform = true;
                    break;
                }
            }
            
            // If there are no platform around the current edge, remove it, and if current is not a platform, remove the edge
            if (!isPlatform && grid[edgePos.x + negativeX][edgePos.y + negativeY].isPlatform == false)
            {
                grid[edgePos.x + negativeX][edgePos.y + negativeY].RemoveEdge();
            }
        }
    }

    // Handle the activation of the trigger where a platform can be built
    private void AddEdges(GridPos _pos)
    {
        foreach (GridPos edgePos in GetEdgesPos(_pos))
        {
            grid[edgePos.x + negativeX][edgePos.y + negativeY].SetAsEdge();
        }
    }
    
    private List<GridPos> GetEdgesPos(GridPos _pos)
    {
        List<GridPos> edgeLocations = new List<GridPos>();
        GridPos firstPos = new GridPos();
        firstPos.x = _pos.x + 1;
        firstPos.y = _pos.y;
        edgeLocations.Add(firstPos);
        GridPos secondPos = new GridPos();
        secondPos.x = _pos.x - 1;
        secondPos.y = _pos.y;
        edgeLocations.Add(secondPos);
        GridPos thirdPos = new GridPos();
        thirdPos.x = _pos.x;
        thirdPos.y = _pos.y + 1;
        edgeLocations.Add(thirdPos);
        GridPos fourthPos = new GridPos();
        fourthPos.x = _pos.x;
        fourthPos.y = _pos.y - 1;
        edgeLocations.Add(fourthPos);
        
        return edgeLocations;
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
                grid[i].Insert(0, buildable);
            }
            negativeY++;
        }
    }
}
