using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Utils.Network
{
    public class RelayHandler
    {
        public RelayServerData? serverData = null;
        
        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                serverData = new RelayServerData(allocation, "dtls");
                MNetworkHandler.Instance.isHost = true;
                MNetworkHandler.Instance.isClient = false;
                
                return joinCode;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        
            return null;
        }
        
        public async Task JoinRelay(string relayCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            
                serverData = new RelayServerData(joinAllocation, "dtls");
                MNetworkHandler.Instance.isHost = false;
                MNetworkHandler.Instance.isClient = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}