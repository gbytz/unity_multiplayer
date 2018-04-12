using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostSetup : NetworkBehaviour {

	public GameObject otherOriginPrefab;

	// Use this for initialization
	public void AddSelf (string phoneID, Vector3 position, Quaternion orientation) {
		CmdAddPhoneOrigin (phoneID, position, orientation);
	}

	[Command]
	void CmdAddPhoneOrigin(string phoneID, Vector3 position, Quaternion orientation){
		
		GameObject newPhone = Instantiate(otherOriginPrefab, position, orientation);
		newPhone.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar(phoneID);

		GameObject.Find("GUI").GetComponent<SceneControl>().Toast("Welcome new player to scene", 2.0f);
	}

	public override void OnStartLocalPlayer()
	{
		GameObject.Find ("GUI").GetComponent<SceneControl> ().thisHostSetup = this;
	}
}
