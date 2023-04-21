using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;


    void Start()
    {
        ShowConnectionPanel();
        GameManager.onGameStateChanged += GameStateChangedCallBack;
    }

    void Update()
    {
        
    }
    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }
    private void GameStateChangedCallBack(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.Game:
                ShowGamePanel();
                break;
            case GameManager.State.Win:
                ShowWinPanel();
                break;
            case GameManager.State.Lose:
                ShowLosePanel();
                break;
        }
    }
    private void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);

        winPanel.SetActive(false);
        losePanel.SetActive(false);

    }
    private void ShowWaitingPanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
    private void ShowGamePanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }
    private void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }
    public void HostButtonCallBack()
    {
        // NetworkManager.Singleton.StartHost();
        ShowWaitingPanel();

        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTransportAndStartNgoAsHost());
    }
    public void ClientButtonCallBack()
    {
        // string ipAdres = IPManager.instance.GetInputIP();

        // UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        // utp.SetConnectionData(ipAdres, 7777);

        // NetworkManager.Singleton.StartClient();

        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTramsportAndStartNgoAsConnectingPlayer());
        ShowWaitingPanel();  
    }
    public void NextButtonCallBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        NetworkManager.Singleton.Shutdown();
    }
    public void PlayButtonCallBack()
    {
        ShowWaitingPanel();

        MatchMakingManager.Instance.PlayBtuttonCallBack();
    }
}
