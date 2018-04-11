using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetID : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.name = Random.Range (0, 1000).ToString();
	}
}
