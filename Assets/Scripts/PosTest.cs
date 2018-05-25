using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTest : MonoBehaviour {

	public Transform track;

	void Update(){
		transform.position = ConvertPhoneToHumanCentroid (track.position);
	}

	private Vector3 ConvertPhoneToHumanCentroid(Vector3 phonePos){
		Vector3 offsetZ = track.TransformPoint(new Vector3 (0f, 0f, -.5f));
		Vector3 offsetY = new Vector3 (0f, -.3f, 0f);

		return offsetZ + offsetY;
	}
}
