using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroy : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider collider){
		Destroy (gameObject);
	}
}
