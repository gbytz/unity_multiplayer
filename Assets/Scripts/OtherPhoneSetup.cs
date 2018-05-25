﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPhoneSetup : MonoBehaviour {

	public string playerID;
	public GameObject phoneGO;
	public GameObject phoneAvatarPrefab;
	private GameObject phoneAvatar;

	public void InitPhoneAvatar (string ID) {

		playerID = ID;
		phoneAvatar = Instantiate (phoneAvatarPrefab);
		GetComponent<AvatarControl> ().thisAvatar = phoneAvatar;
		phoneGO = GameObject.Find(playerID);
		phoneGO.GetComponent<PlayerControl> ().SetGameStarted (gameObject);

		phoneAvatar.GetComponentInChildren<UpdateTransform> ().thisPlayer = phoneGO;


	}

	void Update () {
		phoneAvatar.transform.position = transform.position + transform.rotation * phoneGO.transform.position;
		phoneAvatar.transform.rotation = transform.rotation * phoneGO.transform.rotation;
	}

}
