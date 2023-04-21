using System;
using System.Collections;
using System.Collections.Generic;
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

public class MatchMakingManager : MonoBehaviour
{
    public static MatchMakingManager Instance;

    Lobby lobby;

    [Header("Settings")]
    [SerializeField] private string _joinCode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public async void PlayBtuttonCallBack()
    {
        await Authenticate();

        lobby = await QuickJoinLobby() ?? await CreateLobby();
    }

    async Task Authenticate()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync
                (
                    lobby.Data[_joinCode].Value
                );

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
                (
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData
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
            string lobbyName = "MyLobby";

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.Data = new Dictionary<string, DataObject>
            {
                {_joinCode ,new DataObject(DataObject.VisibilityOptions.Public , joinCode) }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            StartCoroutine(HeartbeatLobbyCoriutine(lobby.Id, 15));

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
        catch (Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
    }
    IEnumerator HeartbeatLobbyCoriutine(string lobbyId,float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

}
