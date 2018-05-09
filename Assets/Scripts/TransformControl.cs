using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TransformControl : NetworkBehaviour
{
	public GameObject tapPrefab;

	public GameObject otherOriginPrefab;
	public SceneControl sceneControl;

	private Vector3 getTapLocalFrame;
	private Vector3 getTapRemoteFrame;
	private Vector3 tapLocalFrame;
	private Vector3 tapRemoteFrame;

	private bool getTap = false;
	private bool tap = false;

	void Start ()
	{
		sceneControl = GameObject.Find ("GUI").GetComponent<SceneControl> ();
	}

	void Update ()
	{

		if (!isLocalPlayer) {
			return;
		}

		if (Input.touchCount > 0 && sceneControl.lookFor.Count > 0) {
			
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
				RaycastHit hit;
				bool hitSomething = Physics.Raycast (ray, out hit, Mathf.Infinity);
				Debug.Log ("Hit " + hitSomething.ToString ());
				if (hitSomething) {
					Debug.Log ("Hit " + hit.collider.name);
				}
						
				if (hitSomething && hit.collider.name == "SU Player(Clone)") {
					Debug.Log ("tapped person");
					Tap (sceneControl.lookFor.Dequeue (), hit.collider.transform.position);
					Destroy (hit.collider.gameObject);
				}
			}
		
		}
	}

	public Vector3 GetTap (GameObject localPlayer, Vector3 localPos)
	{
		Debug.Log ("GotTapped");
		Vector3 remotePos = transform.position;
		getTapLocalFrame = localPos;
		getTapRemoteFrame = remotePos;
		getTap = true;

		if (tap) {
			InitOrigin ();
			localPlayer.GetComponent<PlayerControl> ().SetGameStarted ();
		} else {
			getTap = true;
		}

		return getTapRemoteFrame;
	}

	//TODO: remove
	public void TestTap ()
	{
		Tap (sceneControl.lookFor.Dequeue (), new Vector3 (0, 0, 1));
	}

	private void Tap (string otherID, Vector3 tapLocalFrame)
	{
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();

		tapRemoteFrame = otherTC.GetTap (gameObject, tapLocalFrame);

		CmdRemoteTap (otherID, tapLocalFrame, tapRemoteFrame);

		sceneControl.UpdateLookForDisplay ();

	}

	[Command]
	public void CmdRemoteTap (string otherID, Vector3 tapRemote, Vector3 tapLocal)
	{
		RpcRemoteTap (otherID, tapRemote, tapLocal);
	}

	[ClientRpc]
	public void RpcRemoteTap (string otherID, Vector3 tapRemote, Vector3 tapLocal)
	{
		GameObject otherPlayer = GameObject.Find (otherID);
		TransformControl otherTC = otherPlayer.GetComponent<TransformControl> ();
		if (!otherTC.isLocalPlayer) {
			return;
		}

		this.tapRemoteFrame = tapRemote;
		this.tapLocalFrame = tapLocal;

		if (getTap) {
			InitOrigin ();
			otherPlayer.GetComponent<PlayerControl> ().SetGameStarted ();
		} else {
			tap = true;
		}
	}

	private void InitOrigin ()
	{

		//TODO: assume same height
		getTapLocalFrame.y = 0;
		getTapRemoteFrame.y = 0;
		tapLocalFrame.y = 0;
		tapRemoteFrame.y = 0;

		Vector3 localVector = getTapLocalFrame - tapLocalFrame;
		Vector3 remoteVector = getTapRemoteFrame - tapRemoteFrame;

		float angleRemoteToLocal = Vector3.SignedAngle (remoteVector, localVector, Vector3.up);
		Vector3 offsetLocalToRemote =  getTapLocalFrame - Quaternion.AngleAxis (angleRemoteToLocal, Vector3.up) * getTapRemoteFrame;

		GameObject thisOrigin = Instantiate (otherOriginPrefab, offsetLocalToRemote, Quaternion.Euler (0, angleRemoteToLocal, 0));
		thisOrigin.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar (name);

		GetComponent<PlayerControl> ().SetGameStarted (thisOrigin);

		print ("Init Origin");
	}

}
