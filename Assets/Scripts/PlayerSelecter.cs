using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSelecter : NetworkBehaviour
{
    public static PlayerSelecter Instance;

    private bool isHostTurn;
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

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }
    void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
        {
            return;
        }
        GameManager.onGameStateChanged += GameStateChangedCallBack;
        Egg.onHit += SwitchPlayer;
    }
  
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
        Egg.onHit -= SwitchPlayer;

    }
    private void GameStateChangedCallBack(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.Game:
                Initiliaze();
                break;
        }
    }
    void SwitchPlayer()
    {
        isHostTurn = !isHostTurn;

        Initiliaze();
    }
  
    void Initiliaze()
    {
        PlayerStateManager[] playerStateManagers = FindObjectsOfType<PlayerStateManager>();

        for (int i = 0; i < playerStateManagers.Length; i++)
        {
            if (playerStateManagers[i].GetComponent<NetworkObject>().IsOwnedByServer)
            {
                if (isHostTurn)
                {
                    playerStateManagers[i].Enable();
                }
                else
                {
                    playerStateManagers[i].Disable();
                }
            }
            else
            {
                if (isHostTurn)
                {
                    playerStateManagers[i].Disable();
                }
                else
                {
                    playerStateManagers[i].Enable();
                }  
            }
        }
    }
    public bool GetIsHostTurn()
    {
        return isHostTurn;
    }
}
    


