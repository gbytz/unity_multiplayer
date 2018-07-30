using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DefenseControl : NetworkBehaviour {

	private PlayerController playerController;
	private Jido_Player transformControl;
	public GameObject defensePrefab;

	// Use this for initialization
	void Start () {
		playerController = GetComponent<PlayerController> ();
		transformControl = GetComponent<Jido_Player> ();
	}

	void Update ()
	{
		if (!isLocalPlayer) {
			return;
		}

		if (Input.touchCount > 0 && playerController.gameStarted ) {

			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
				RaycastHit hit;
				bool hitSomething = Physics.Raycast (ray, out hit, Mathf.Infinity);

                if (hitSomething && hit.collider.GetComponent<DetectedObjectControl>()) {
					Vector3 pos = hit.collider.transform.position;
					Instantiate (defensePrefab, pos, Quaternion.identity);
					CmdPlaceDetectedObject (pos);
					Destroy (hit.collider.gameObject);
				}
			}

		}
	}

	[Command]
	public void CmdPlaceDetectedObject(Vector3 position){
		RpcPlaceDetectedObject (position);
	}

	[ClientRpc]
	public void RpcPlaceDetectedObject(Vector3 position){
		if (isLocalPlayer) {
			return;
		}
		Instantiate (defensePrefab, transformControl.GetLocalPosition (position), Quaternion.identity);
	}


}
