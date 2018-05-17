//Sets up and controls the remote player's local version. Sits on the remote player's local origin.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarControl))]
public class OtherPhoneSetup : MonoBehaviour {

	public string playerID;
	private GameObject phoneGO; //Networked remote player's GO
	public GameObject phoneAvatarPrefab; //This players avatar prefab
	private GameObject phoneAvatar; //This avatar instance

	//Called by TransformControl
	public void InitPhoneAvatar (string ID) {

		playerID = ID;
		phoneAvatar = Instantiate (phoneAvatarPrefab);
		GetComponent<AvatarControl> ().thisAvatar = phoneAvatar;
		phoneGO = GameObject.Find(playerID);
		phoneGO.GetComponent<PlayerControl> ().SetGameStarted (gameObject);

	}
	
	// Sets the remote player's local avatar's position and rotation using the local origin
	void Update () {
		phoneAvatar.transform.position = transform.position + transform.rotation * phoneGO.transform.position;
		phoneAvatar.transform.rotation = transform.rotation * phoneGO.transform.rotation;
	}
		
}
