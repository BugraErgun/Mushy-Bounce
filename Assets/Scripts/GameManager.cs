using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public static Action<State> onGameStateChanged;
    public enum State
    {
        Menu,
        Game,
        Win,
        Lose
    }

    private State gameState;

    public int connectedPlayers = 0;

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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }
    public override void OnDestroy()
    {
        base .OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }

    private void Start()
    {
        gameState = State.Menu;
    }
    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
        {
            return;
        }

        connectedPlayers++;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
         connectedPlayers++;

        if (connectedPlayers >= 2)
        {
            StartGame();
        }
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
