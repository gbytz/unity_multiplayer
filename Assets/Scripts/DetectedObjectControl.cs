using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedObjectControl : MonoBehaviour {

	public GameObject nameGO;
	public float startTime;
	public bool isVisible = false;

	void Start(){
		startTime = Time.fixedTime;
		Invoke ("DestroySelf", 0.5f);

		if (isVisible) {
			Invoke ("TurnOnMesh", 0.1f);
		}
	}

	public void TurnOnMesh(){
		GetComponent<MeshRenderer> ().enabled = true;
		nameGO.SetActive (true);
	}

	public void SetName(string name){
		nameGO.GetComponent<TextMesh>().text = name;
		gameObject.name = name;
	}

	void OnTriggerEnter(Collider other){
		if (other.name == name) {

			if (other.GetComponent<DetectedObjectControl> ().startTime < startTime) {
				transform.position = (transform.position + other.transform.position) / 2;
				transform.localScale = (transform.localScale + other.transform.localScale) / 2;
				Destroy (other.gameObject);
			}
		}
	}

	private void DestroySelf(){
		Destroy (gameObject);
	}
		
}
