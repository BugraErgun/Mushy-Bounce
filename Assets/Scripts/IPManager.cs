using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Linq;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class IPManager : MonoBehaviour
{
    public static IPManager instance;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI ipText;
    [SerializeField] private TMP_InputField ipInputField;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

        }
    }
    void Start()
    {
        ipText.text = GetLocalIP();

        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(GetLocalIP(), 7777);

    }

    void Update()
    {
        
    }
    private string GetLocalIP()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).
            AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }
    public string GetInputIP()
    {
        return ipInputField.text;
    }
}
