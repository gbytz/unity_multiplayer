using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTransform : MonoBehaviour {

	public GameObject thisPlayer;
	public TransformControl localTC;

	void Start(){
		localTC = GameObject.Find ("GUI").GetComponent<SceneControl> ().localPlayer.GetComponent<TransformControl> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.name == "SU Player(Clone)") {
			localTC.AutoTap (thisPlayer.name, other.transform.position);
		}
	}

	void OnTriggerStay(Collider other){
		if (other.name == "SU Player(Clone)") {
			localTC.AutoTap (thisPlayer.name, other.transform.position);
		}
	}

}
