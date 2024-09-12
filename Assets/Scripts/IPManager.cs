using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class IPManager : MonoBehaviour
{
    public static IPManager instance;

    [SerializeField] private TextMeshProUGUI ipText;
    [SerializeField] private TMP_InputField ipInputField;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        ipText.text = GetLocalIP();

        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(GetLocalIP(), 7777);
            
    }

    private string GetLocalIP()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }

    public string GetInputIP()
    {
        return ipInputField.text;
    }
}
