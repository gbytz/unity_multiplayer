using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPhoneSetup : MonoBehaviour {

	public string playerID;
	private Transform phoneTransform;
	public GameObject phoneAvatarPrefab;
	private GameObject phoneAvatar;

	// Use this for initialization
	public void InitPhoneAvatar (string ID) {

		playerID = ID;
		phoneAvatar = Instantiate (phoneAvatarPrefab);
		phoneTransform = GameObject.Find(playerID).transform;

	}
	
	// Update is called once per frame
	void Update () {
		phoneAvatar.transform.position = transform.position + phoneTransform.position;
		phoneAvatar.transform.rotation = transform.rotation * phoneTransform.rotation;
	}
		
}
