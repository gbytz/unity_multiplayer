using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	public float destroyTime = 0.5f;

	// Use this for initialization
	void Start () {
		Invoke ("SelfDestruct", destroyTime);
	}
	
	// Update is called once per frame
	public void SelfDestruct () {
		Destroy (gameObject);
	}
}
