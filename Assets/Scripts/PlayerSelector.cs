using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerSelector : NetworkBehaviour
{
    public static PlayerSelector Instance { get; private set; }

    private bool isHostTurn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
        {
            return;
        }

        GameManager.onGameStateChanged += GameStateChangedCallBack;
        Egg.onHit += SwitchPlayers;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
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
    private void SwitchPlayers()
    {
        isHostTurn = !isHostTurn;

        Initiliaze();
    }
    private void Initiliaze()
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

    public bool IsHostTurn()
    {
        return isHostTurn;
    }
}
