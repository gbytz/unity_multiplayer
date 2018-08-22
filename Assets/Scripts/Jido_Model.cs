using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jido_Model : MonoBehaviour {

	private Jido_Player thisTransformControl;
	private Jido_Player localPlayerTransformControl;

	[HideInInspector]
	public string playerID;

	void Start(){
		localPlayerTransformControl = FindObjectOfType<Jido_Manager> ().LocalPlayerReference.GetComponent<Jido_Player> ();
		playerID = transform.parent.name; 
		thisTransformControl = GameObject.Find(playerID).GetComponent<Jido_Player>();
		InvokeRepeating("CheckSeen", 1f, 0.3f);
	}

	void CheckSeen(){
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
		{
			thisTransformControl.ModelWasSeen();
		}
	}

	public void UpdateTransform(Vector3 detectedPosition){
		localPlayerTransformControl.AutoTap (playerID, detectedPosition);
	}
}
