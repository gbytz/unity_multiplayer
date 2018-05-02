using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("SelfDestroy", 0.02f);
	}
	
	// Update is called once per frame
	public void SelfDestruct () {
		Destroy (gameObject);
	}
}
