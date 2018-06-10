using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTransform : MonoBehaviour {

	public GameObject thisPlayer;
	public TransformControl localTC;

	void Start(){
        localTC = FindObjectOfType<GameManager> ().LocalPlayerReference.GetComponent<TransformControl> ();
	}

	void OnTriggerEnter(Collider other){
        if (other.GetComponent<DetectedObjectControl>() != null) {
			localTC.AutoTap (thisPlayer.name, other.transform.position);
		}
	}

	void OnTriggerStay(Collider other){
        if (other.GetComponent<DetectedObjectControl>() != null) {
			localTC.AutoTap (thisPlayer.name, other.transform.position);
		}
	}
}
