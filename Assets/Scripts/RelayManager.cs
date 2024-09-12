using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;
using Unity.VisualScripting;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class RelayManager : MonoBehaviour
{
	public static RelayManager Instance;

    const int m_Maxconnetions = 1;
    public string relayJoinCode;

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
    private void Start()
    {
		AuthenticationgAPlayer();
    }
    async void AuthenticationgAPlayer()
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

	public async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections,string region = null)
	{
		Allocation allocation;
		string createJoinCode;

		try
		{
			allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
		}
		catch (Exception e)
		{
			throw;	
		}

		try
		{
			createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			joinCodeText.text = createJoinCode;
			Debug.Log(joinCodeText.text);
		}
		catch (Exception e)
		{

			throw;
		}
		return new RelayServerData(allocation, "dtls"); 
	}

	public IEnumerator ConfigureTransportAndStartNgoHost()
	{
		var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_Maxconnetions);
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
			allocation = await RelayService.Instance.JoinAllocationAsync(joinCodeText.text);
		}
		catch
		{
			throw;
		}

		return new RelayServerData(allocation, "dtsl");
	}
	public IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer()
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
