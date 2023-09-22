using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network.Data
{
    public class BoatData : NetworkData<Components.Data.Boat>
    {
        private NetworkList<(Vector3Int coords, Components.Data.Block block)> grid;

        private void Awake()
        {
            grid = new NetworkList<(Vector3Int coords, Components.Data.Block block)>();
        }

        public override Components.Data.Boat GetValue()
        {
            
            var boat = new Components.Data.Boat();
            foreach (var block in grid)
            {
                boat.SetBlock(block.coords, block.block);
            }
            return boat;
        }
        
        #region SetBlock
        
        public void SetBlock(Vector3Int coords, Components.Data.Block item)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _SetBlock(coords, item);
            }
            else
            {
                SetBlockServerRpc(coords, item);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetBlockServerRpc(Vector3Int index, Components.Data.Block item)
        {
            _SetBlock(index, item);
        }
        
        private void _SetBlock(Vector3Int index, Components.Data.Block item)
        {
            if (grid.Contains((index, item)))
            {
                grid[grid.IndexOf((index, item))] = (index, item);
            }
            else
            {
                grid.Add((index, item));
            }
        }
        
        #endregion
        
        #region RemoveBlock
        
        public void RemoveBlock(Vector3Int coords)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _RemoveBlock(coords);
            }
            else
            {
                RemoveBlockServerRpc(coords);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemoveBlockServerRpc(Vector3Int coords)
        {
            _RemoveBlock(coords);
        }
        
        public void _RemoveBlock(Vector3Int coords)
        {
            foreach (var block in grid)
            {
                if (block.coords == coords)
                {
                    grid.Remove(block);
                    break;
                }
            }
        }
        
        #endregion
    }
}