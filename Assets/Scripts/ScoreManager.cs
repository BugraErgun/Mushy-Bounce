using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private int hostScore;
    [SerializeField] private int clientScore;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
            return;

        Egg.onFellInWater += Egg_FellInWater;

        GameManager.onGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.State state)
    {
        switch (state)
        {
      
            case GameManager.State.Game:
                ResetScore();
                break;
         
        }
    }

    private void ResetScore()
    {
        hostScore = 0;
        clientScore = 0;

        UpdateScoreClientRPC(hostScore,clientScore);
        UpdateScoreText();
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void Egg_FellInWater()
    {
        if (PlayerSelector.Instance.IsHostTurn())
        {
            clientScore++;
        }
        else
        {
            hostScore++;
        }
        UpdateScoreClientRPC(hostScore,clientScore);
        UpdateScoreText();

        CheckForEndGame();
    }

    private void CheckForEndGame()
    {
        if (hostScore >= 3)
        {
            HostWin();
        }
        else if (clientScore >= 3)
        {
            ClientWin();
        }
        else
        {
            ReuseEgg();
        }
    }

    private void ClientWin()
    {
        ClientWinClientRPC();
    }
    [ClientRpc]
    private void ClientWinClientRPC()
    {
        if (IsServer)
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
        else
        {
            GameManager.Instance.SetGameState(GameManager.State.Win);
        }
    }

    private void HostWin()
    {
        HostWinClientRPC();
    }
    [ClientRpc]
    private void HostWinClientRPC()
    {
        if (IsServer)
        {
            GameManager.Instance.SetGameState(GameManager.State.Win);
        }
        else
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
    }

    private void ReuseEgg()
    {
        EggManager.instance.ReuseEgg();
    }

    [ClientRpc]
    private void UpdateScoreClientRPC(int hostScore,int clientScore)
    {
        this.hostScore = hostScore;
        this.clientScore = clientScore;
    }

    private void UpdateScoreText()
    {
        UpdateScoreTextClientRPC();

    }

    [ClientRpc]
    private void UpdateScoreTextClientRPC()
    {
        scoreText.text = "<color=#0088FF>" + hostScore + "</color> - <color=#CF2A2A>" + clientScore + "</color>";

    }

    public override void OnDestroy()
    {
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        Egg.onFellInWater -= Egg_FellInWater;
        GameManager.onGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
