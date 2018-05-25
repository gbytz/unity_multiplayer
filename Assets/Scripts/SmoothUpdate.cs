using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothUpdate : MonoBehaviour {

	private Vector3 posTarget;
	private Quaternion rotTarget;
	private bool updateTarget;
	private float speed = 2.0f;


	void Update(){
		if (updateTarget) {
			transform.position = Vector3.Lerp (transform.position, posTarget, Time.deltaTime * speed);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotTarget, Time.deltaTime * speed);
		}
	}

	public void SetTarget(Vector3 pos, Quaternion rot){
		posTarget = pos;
		rotTarget = rot;
		updateTarget = true;
	}
}
