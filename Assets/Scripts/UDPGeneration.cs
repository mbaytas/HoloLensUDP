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
			var bytes = System.Text.Encoding.UTF8.GetBytes(DataString);

			// #if is required because SendUDPMessage() is async
			#if !UNITY_EDITOR
			comm.SendUDPMessage(incomingIP, comm.externalPort, data);
			#endif
		}
	}
}
