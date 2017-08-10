using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPResponse : MonoBehaviour {

	public TextMesh textMesh = null;

	void Start () {
	
	}

	void Update () {
	
	}

	public void ResponseToUDPPacket(string incomingIP, string incomingPort, byte[] data)
	{
		string dataString = System.Text.Encoding.UTF8.GetString (data);

		// Example: Write
		if (textMesh != null) {
			textMesh.text = dataString;
		}

		#if !UNITY_EDITOR

		// // Example: Echo 
		// UDPCommunication comm = UDPCommunication.Instance;
		// comm.SendUDPMessage(incomingIP, comm.externalPort, data);

		#endif
	}
}