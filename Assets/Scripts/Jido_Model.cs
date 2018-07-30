using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jido_Model : MonoBehaviour {

	private Jido_Player localTC;
	public string playerID;

	void Start(){
		localTC = FindObjectOfType<Jido_Manager> ().LocalPlayerReference.GetComponent<Jido_Player> ();
		playerID = transform.parent.name; 
		InvokeRepeating("CheckSeen", 1f, 0.3f);
	}

	void CheckSeen(){
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
		{
			localTC.ModelWasSeen();
		}
	}

	public void UpdateTransform(Vector3 detectedPosition){
		localTC.AutoTap (playerID, detectedPosition);
	}
}
