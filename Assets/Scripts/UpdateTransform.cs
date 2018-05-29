using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTransform : MonoBehaviour {

	public GameObject thisPlayer;
	public TransformControl localTC;

	void Start(){
        localTC = FindObjectOfType<SceneControl> ().localPlayer.GetComponent<TransformControl> ();
	}

	void OnTriggerEnter(Collider other){
		print ("Ship Collided before: " + thisPlayer.name);
		if (other.name == "SU Player(Clone)") {
			print ("Ship Collided: " + thisPlayer.name);
			localTC.AutoTap (thisPlayer.name, other.transform.position);
		}
	}
		 
}
