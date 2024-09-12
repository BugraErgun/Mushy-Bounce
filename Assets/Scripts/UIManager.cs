using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    private void Start()
    {
        ShowConnectionPanel();

        GameManager.onGameStateChanged += GameStateChangedCallBack;
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
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }
    private void ShowGamePanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private void ShowWinPanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }
    private void ShowLosePanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void HostBtnCallBacks()
    {
        //NetworkManager.Singleton.StartHost();
        ShowWaitingPanel();

        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTransportAndStartNgoHost());
    }

    public void ClientBtnCallBacks()
    {
        // string ipAdress = IPManager.instance.GetInputIP();

        // UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        // utp.SetConnectionData(ipAdress, 7777);

        // NetworkManager.Singleton.StartClient();

        RelayManager.Instance.StartCoroutine(RelayManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer());

        ShowWaitingPanel();
    }

    public void NextBtnCallBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        NetworkManager.Singleton.Shutdown();
    }

    public void PlayBtnCallBack()
    {
        ShowWaitingPanel();

        MatchMakingManager.Instance.PlayBtnCallBack();
    }
}
