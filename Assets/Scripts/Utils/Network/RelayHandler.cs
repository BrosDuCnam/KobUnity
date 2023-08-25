using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Utils.Network
{
    public class RelayHandler
    {
        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                RelayServerData serverData = new RelayServerData(allocation, "dtls");
                MNetwork.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
                MNetwork.Singleton.StartHost();
            
                return joinCode;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        
            return null;
        }
        
        public async void JoinRelay(string relayCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            
                RelayServerData serverData = new RelayServerData(joinAllocation, "dtls");
                MNetwork.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
                MNetwork.Singleton.StartClient();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}