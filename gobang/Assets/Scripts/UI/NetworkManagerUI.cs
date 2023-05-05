using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Text;
using AddressFamily = System.Net.Sockets.AddressFamily;

public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI Instance { get; set; }

    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button shutBtn;
    [SerializeField] private Button TBtn;

    private string playerIP;

    private void Awake()
    {
        Instance = this;
        /*
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                playerIP = ip.ToString();
                break;
            }
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            playerIP,  // The IP address is a string
            (ushort)7777 // The port number is an unsigned short
        );
        */

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });

        serverBtn.onClick.AddListener(() =>{
            NetworkManager.Singleton.StartServer();
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();

        });

        shutBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        });

        TBtn.onClick.AddListener(() => {
            //NetworkConnecter.Instance.addPlayerServerRpc();
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        });
    }
    public void ConnectToIP(string targetIP)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            targetIP,  // The IP address is a string
            (ushort)7777 // The port number is an unsigned short
        );
        NetworkManager.Singleton.StartClient();
    }


}
