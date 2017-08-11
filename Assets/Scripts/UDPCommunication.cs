using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using HoloToolkit.Unity;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#endif

[System.Serializable]
public class UDPMessageEvent : UnityEvent<string, string, byte[]>
{

}

public class UDPCommunication : MonoBehaviour
{
	[Tooltip ("Port to open on HoloLens to send or listen")]
	public string internalPort = "11000";

	[Tooltip("IP address to send to")]
	public string externalIP = "192.168.1.130";

	[Tooltip("Port to send to")]
	public string externalPort = "11000";

	[Tooltip("Send a message on startup")]
	public bool sendPingAtStart = true;

	[Tooltip("Contents of the startup message")]
	public string PingMessage = "Let there be UDP.";

	[Tooltip("Functions to invoke on packet reception")]
	public UDPMessageEvent udpEvent = null;

	private readonly  Queue<Action> ExecuteOnMainThread = new Queue<Action>();


	#if !UNITY_EDITOR

	//we've got a message (data[]) from (host) in case of not assigned an event
	void UDPMessageReceived(string host, string port, byte[] data)
	{
	Debug.Log("UDP message from " + host + " on port " + port + ", " + data.Length.ToString() + " bytes ");
	}

	//Send a UDP-Packet
	public async void SendUDPMessage(string HostIP, string HostPort, byte[] data)
	{
	await _SendUDPMessage(HostIP, HostPort, data);
	}



	DatagramSocket socket;

	async void Start()
	{
	if (udpEvent == null)
	{
	udpEvent = new UDPMessageEvent();
	udpEvent.AddListener(UDPMessageReceived);
	}


	Debug.Log("Waiting for a connection...");

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
	Debug.Log(e.ToString());
	Debug.Log(SocketError.GetStatus(e.HResult).ToString());
	return;
	}

	if (externalIP != null && externalPort != null && sendPingAtStart)
	{
	if (PingMessage == null){
	PingMessage = "";
	}
	SendUDPMessage(externalIP, externalPort, Encoding.UTF8.GetBytes(PingMessage));
	}

	}




	private async System.Threading.Tasks.Task _SendUDPMessage(string externalIP, string externalPort, byte[] data)
	{
	using (var stream = await socket.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPort))
	{
	using (var writer = new Windows.Storage.Streams.DataWriter(stream))
	{
	writer.WriteBytes(data);
	await writer.StoreAsync();

	}
	}
	}


	#else
	// to make Unity-Editor happy :-)
	void Start()
	{

	}

	public void SendUDPMessage(string HostIP, string HostPort, byte[] data)
	{

	}

	#endif


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

	#if !UNITY_EDITOR
	private void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
	Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
	{
	Debug.Log("GOT MESSAGE FROM: " + args.RemoteAddress.DisplayName);
	//Read the message that was received from the UDP  client.
	Stream streamIn = args.GetDataStream().AsStreamForRead();
	MemoryStream ms = ToMemoryStream(streamIn);
	byte[] msgData = ms.ToArray();


	if (ExecuteOnMainThread.Count == 0)
	{
	ExecuteOnMainThread.Enqueue(() =>
	{
	Debug.Log("ENQEUED ");
	if (udpEvent != null)
	udpEvent.Invoke(args.RemoteAddress.DisplayName, internalPort, msgData);
	});
	}
	}


	#endif
}
