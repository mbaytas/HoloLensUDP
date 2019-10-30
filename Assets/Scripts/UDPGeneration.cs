using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPGeneration : MonoBehaviour {

	public GameObject UDPCommGameObject;

	public string DataString = "UDP is real.";

	void Start () {
		if (UDPCommGameObject == null) {
			Debug.Log ("ERR UDPGEN: UDPSender is required. Self-destructing.");
			Destroy (this);
		}	
	}
	
	void Update () {
		if (DataString != null) {
			// UTF-8 is real
			var dataBytes = System.Text.Encoding.UTF8.GetBytes(DataString);
			UDPCommunication comm = UDPCommGameObject.GetComponent<UDPCommunication> ();

			// #if is required because SendUDPMessage() is async
			//UPDATE: #if is handled in UDPCommunication.cs
			comm.SendUDPMessage(comm.externalIP, comm.externalPort, dataBytes);
		}
	}
}
