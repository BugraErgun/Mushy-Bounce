using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class ScoreManager : NetworkBehaviour
{

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private int hostScore;
    private int clientScore;

    private void Start()
    {
        UpdateScoreText();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }
    private void NetworkManager_OnServerStarted()
    {
        if (!IsServer)
        {
            return;
        }
        Egg.onFellInWater += EggOnFellInWaterCallBack;
        GameManager.onGameStateChanged += GameStateChangedCallBack;
    }
    private void EggOnFellInWaterCallBack()
    {
        if (PlayerSelecter.Instance.GetIsHostTurn())
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
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        Egg.onFellInWater -= EggOnFellInWaterCallBack;
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }
    [ClientRpc]
    private void UpdateScoreClientRPC(int hostScore, int clientScore)
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
        scoreText.text = "<color=#00A7FF>" + hostScore + "</color>" + "-" + "<color=#FF2E00>" + clientScore + "</color>";
    }
    private void GameStateChangedCallBack(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.Game:
                ResetScores();
                break;
        }
    }
    private void ResetScores()
    {
        hostScore = 0;
        clientScore = 0;
        UpdateScoreClientRPC(hostScore, clientScore);
        UpdateScoreText();

    }
    private void CheckForEndGame()
    {
        if (hostScore >= 3)
        {
            HostWin();
        }
        else if(clientScore >=3)
        {
            ClientWin();
        }
        else
        {
            ReuseEgg();
        }
    }
    private void ReuseEgg()
    {
        EggManager.instance.ReuseEgg();
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
}
