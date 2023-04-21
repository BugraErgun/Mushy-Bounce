using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public enum State
    {
        Menu,
        Game,
        Win,
        Lose
    }
    private State gameState;

    private int connectedPlayers;

    [Header("Evenets")]
    public static Action<State> onGameStateChanged;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
        
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;


    }
    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
        {
            return;
        }

        connectedPlayers++;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;   
    }
    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        connectedPlayers++;

        if (connectedPlayers >=2)
        {
            StartGame();
        }
    }
    void Start()
    {
        gameState = State.Menu;

    }

    private void StartGame()
    {
        StartGameClientRPC();
    }

    [ClientRpc]
    private void StartGameClientRPC()
    {
        gameState = State.Game;
        onGameStateChanged?.Invoke(gameState);
    }
    public void SetGameState(State state)
    {
        this.gameState = state;
        onGameStateChanged?.Invoke(state);
    }
}
