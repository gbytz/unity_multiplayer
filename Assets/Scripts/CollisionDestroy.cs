using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroy : MonoBehaviour {

    public List<string> TagsToIgnore = new List<string>();

	// Use this for initialization
	void OnTriggerEnter(Collider collider){
        if(!TagsToIgnore.Contains(collider.tag))
		    Destroy (gameObject);
	}
}
