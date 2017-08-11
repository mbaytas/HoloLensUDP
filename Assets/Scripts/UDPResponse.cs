using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPResponse : MonoBehaviour {

	public TextMesh textMesh = null;

	void Start () {
	
	}

	void Update () {
	
	}

	public void ResponseToUDPPacket(string fromIP, string fromPort, byte[] data)
	{
		string dataString = System.Text.Encoding.UTF8.GetString (data);

		if (textMesh != null) {
			textMesh.text = dataString;
		}
	}
}