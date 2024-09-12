using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class MatchMakingManager : MonoBehaviour
{
    public static MatchMakingManager Instance;

    Lobby lobby;

    [SerializeField] private string _joinCode;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    async Task AuthenticationgAPlayer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerIOd = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void PlayBtnCallBack()
    {
        await AuthenticationgAPlayer();

        lobby = await QuickJoinLobby() ?? await CreateLobby();
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[_joinCode].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
                (
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        } 
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            int maxPlayers = 2;
            string lobbyName = "My Lobby";

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.Data = new Dictionary<string, DataObject> { { _joinCode, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData
                (
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();

            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId,float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
