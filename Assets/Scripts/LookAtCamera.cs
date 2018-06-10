using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	GameObject mainCamera;

	void Start() {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	void Update () {
		
		Vector3 targetPos = mainCamera.transform.position;
		targetPos.y = transform.position.y;
		transform.LookAt (targetPos);

	}
}
