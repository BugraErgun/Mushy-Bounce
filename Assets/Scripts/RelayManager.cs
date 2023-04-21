using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode.Transports;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;
using TMPro;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    const int m_MAXCONNECTIONS = 1;
    public string relayJoinCode;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;

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
    void Start()
    {
        AuthenticatingPlayer();
    }

    void Update()
    {
        
    }
    async void AuthenticatingPlayer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    public async Task<RelayServerData> AllLocateRelayServerAndGetJoinCode(int maxConnections , string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.Log($"RELAY CREATRE ALLOCATION REQUEST FAILED{e.Message}");
            throw;
        }
        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = createJoinCode;
        }
        catch
        {
            throw;
        }
        return new RelayServerData(allocation, "dtls");
    }
    public IEnumerator ConfigureTransportAndStartNgoAsHost()
    {
        var serverRelayUtilityTask = AllLocateRelayServerAndGetJoinCode(m_MAXCONNECTIONS);
        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if (serverRelayUtilityTask.IsFaulted)
        {
            yield break;
        }
        var relayServerData = serverRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
        yield return null;
    }
    public async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch 
        {
            throw;
        }
        return new RelayServerData(allocation, "dtls");
    }
    public IEnumerator ConfigureTramsportAndStartNgoAsConnectingPlayer()
    {
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(joinCodeInputField.text);

        while (!clientRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if (clientRelayUtilityTask.IsFaulted)
        {
            yield break;
        }
        var relayServerData = clientRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
        yield return null;
    }
}
