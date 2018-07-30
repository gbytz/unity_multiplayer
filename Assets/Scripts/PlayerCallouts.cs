using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCallouts : MonoBehaviour {

	public GameObject playerModel;
	private float yPos;

	void Update(){
		if (playerModel != null)
		{
			Vector3 playerPos = playerModel.transform.TransformPoint(new Vector3(0f, 0f, -.4f));
			Vector3 pos = new Vector3(playerPos.x, yPos, playerPos.z);
			float rotY = playerModel.transform.rotation.eulerAngles.y;

			transform.position = pos;
			transform.rotation = Quaternion.Euler(0, rotY, 0);
		}

	}

	public void StartTracking(float floorPos = -1.2f){
		transform.parent = null;

		transform.rotation = Quaternion.identity;
		yPos = floorPos;

		Vector3 playerPos = playerModel.transform.position;
		Vector3 pos = new Vector3(playerPos.x, yPos, playerPos.z);

		transform.position = pos;
	}
}
