using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FollowCamera : NetworkBehaviour {

	private Transform cameraTransform;

	void Start () {
		cameraTransform = GameObject.Find ("Main Camera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		transform.position = cameraTransform.position;
		transform.rotation = cameraTransform.rotation;
	}
}