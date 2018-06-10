using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroy : MonoBehaviour {

    public string TagToIgnore;

	// Use this for initialization
	void OnTriggerEnter(Collider collider){
        if(collider.tag != TagToIgnore)
		    Destroy (gameObject);
	}
}
