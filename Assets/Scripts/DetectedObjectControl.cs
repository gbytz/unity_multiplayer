using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedObjectControl : MonoBehaviour {
	public float startTime;
	public bool isVisible = false;
	public MeshRenderer setColorMaterial;
	public Color colorSet;
	private Vector3 updatePosition;
	private Vector3 updateScale;
	private bool updateFlag = false;

	void Start(){
		startTime = Time.fixedTime;

		InvokeRepeating ("DestroySelf", 1.0f, 1.0f);

		if (isVisible) {
			Invoke ("TurnOnMesh", 0.01f);
		}
	}

	void Update(){
		if (updateFlag) {
			transform.position = Vector3.Lerp (transform.position, updatePosition, Time.deltaTime * 2.0f);
			transform.localScale = Vector3.Lerp (transform.localScale, updateScale, Time.deltaTime * 2.0f);
		}
	}

	public void TurnOnMesh(){
		GetComponent<MeshRenderer> ().enabled = true;
	}

	void OnTriggerEnter(Collider other){
		if (other.name == name) {
			if (other.GetComponent<DetectedObjectControl> ().startTime < startTime) {
				updatePosition = (transform.position + other.transform.position) / 2;
				updateScale = (transform.localScale + other.transform.localScale) / 2;
				//GameObject temp = Instantiate(dissipateBox, updatePosition, transform.rotation) as GameObject;
				//temp.transform.localScale = updateScale;
				updateFlag = true;
				Destroy (other.gameObject);
			}
		}
	}

	private void DestroySelf(){
		if(!updateFlag){
			Destroy(gameObject);
		} else {
			updateFlag = false;
		}
	}
}
