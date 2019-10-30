using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using HoloToolkit.Unity;
using System.Threading;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#else
using System.Net;
using System.Net.Sockets;
#endif

[System.Serializable]
public class UDPMessageEvent : UnityEvent<string, string, byte[]>
{

}
public class UDPCommunication : MonoBehaviour
{
    [Tooltip("Port to open on HoloLens to send or listen")]
    public string internalPort = "11000";

    [Tooltip("IP address to send to")]
    //public string externalIP = "192.168.1.130";
    public string externalIP = "192.168.2.20";
    //public string externalIP = "10.20.1.4";

    [Tooltip("Port to send to")]
    public string externalPort = "5394";

    [Tooltip("Send a message on startup")]
    public bool sendPingAtStart = true;

    [Tooltip("Contents of the startup message")]
    public string PingMessage = "Let there be UDP.";

    [Tooltip("Functions to invoke on packet reception")]
    public UDPMessageEvent udpEvent = null;

    private readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();


    //Default receive method: we've got a message (data[]) from (host) in case of not assigned an event
    void UDPMessageReceived(string host, string port, byte[] data)
    {
        Debug.Log("UDP message from " + host + " on port " + port + ", " + data.Length.ToString() + " bytes ");
    }

#if !UNITY_EDITOR
    DatagramSocket socket;
#else
    UdpClient socket_unity;
    Thread receiveThread;
    IPEndPoint remoteEndPoint;
#endif

    //Send a UDP-Packet
    public async void SendUDPMessage(string HostIP, string HostPort, byte[] data)
    {
        await _SendUDPMessage(HostIP, HostPort, data);
    }



    async void Start()
    {
        if (udpEvent == null)
        {
            udpEvent = new UDPMessageEvent();
            udpEvent.AddListener(UDPMessageReceived);
        }

#if !UNITY_EDITOR
        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;
        HostName IP = null;


        try
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
            hn =>
            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
            == icp.NetworkAdapter.NetworkAdapterId);
            await socket.BindEndpointAsync(IP, internalPort);
        }
        catch (Exception e)
        {
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }
#else

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(externalIP), Convert.ToInt32(externalPort));
        socket_unity = new UdpClient(Convert.ToInt32(internalPort));
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
#endif

        if (externalIP != null && externalPort != null && sendPingAtStart)
        {
            Debug.Log(String.Format("ping to {0}:{1}, From: {2}", externalIP, externalPort, internalPort));

            if (PingMessage == null)
            {
                PingMessage = "";
            }

            try
            {
                SendUDPMessage(externalIP, externalPort, Encoding.UTF8.GetBytes(PingMessage));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

    }


    static MemoryStream ToMemoryStream(Stream input)
    {
        try
        {                                         // Read and write in
            byte[] block = new byte[0x1000];       // blocks of 4K.
            MemoryStream ms = new MemoryStream();
            while (true)
            {
                int bytesRead = input.Read(block, 0, block.Length);
                if (bytesRead == 0) return ms;
                ms.Write(block, 0, bytesRead);
            }
        }
        finally { }
    }

    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();

        }
    }
    private async System.Threading.Tasks.Task _SendUDPMessage(string externalIP, string externalPort, byte[] data)
    {
#if !UNITY_EDITOR
        using (var stream = await socket.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPort))
        {
            using (var writer = new Windows.Storage.Streams.DataWriter(stream))
            {
                writer.WriteBytes(data);
                await writer.StoreAsync();

            }
        }
#else
        await socket_unity.SendAsync(data, data.Length, remoteEndPoint);
#endif
    }


#if !UNITY_EDITOR
    private void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
    Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        //Read the message that was received from the UDP  client.
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        MemoryStream ms = ToMemoryStream(streamIn);
        byte[] msgData = ms.ToArray();


        if (ExecuteOnMainThread.Count == 0)
        {
            ExecuteOnMainThread.Enqueue(() =>
            {
                if (udpEvent != null)
                    udpEvent.Invoke(args.RemoteAddress.DisplayName, internalPort, msgData);
            });
        }
    }

#else
    private void ReceiveData()
    {
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] msgData = socket_unity.Receive(ref senderIP);

                string a = System.Text.Encoding.UTF8.GetString(msgData);

                if (ExecuteOnMainThread.Count == 0)
                {                    
                    ExecuteOnMainThread.Enqueue(() =>
                    {
                        if (udpEvent != null)
                            udpEvent.Invoke(senderIP.Address.ToString(), internalPort, msgData);
                    });
                }

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }


    }
    
#endif

    internal void sendPingMsg()
    {
        SendUDPMessage(externalIP, externalPort, Encoding.UTF8.GetBytes(PingMessage));
    }
}
