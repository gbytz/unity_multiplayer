using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jido_Update_Transform : MonoBehaviour {

	private Jido_Transform_Control localTC;
	private string playerID;

	void Start(){
        localTC = FindObjectOfType<GameManager> ().LocalPlayerReference.GetComponent<Jido_Transform_Control> ();

		//TODO: More robust way to get the name of this player object
		playerID = transform.parent.name; 
	}
		
    //If this object is intersecting with a "Detected Object" world object then update transform control
	void OnTriggerEnter(Collider other){
        if (other.GetComponent<DetectedObjectControl>() != null) {
			localTC.AutoTap (playerID, other.transform.position);
		}
	}

	void OnTriggerStay(Collider other){
        if (other.GetComponent<DetectedObjectControl>() != null) {
			localTC.AutoTap (playerID, other.transform.position);
		}
	}
}
