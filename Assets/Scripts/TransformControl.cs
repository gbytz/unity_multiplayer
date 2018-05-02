using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformControl : NetworkBehaviour {

	public GameObject otherOriginPrefab;

	private Vector3 getTapOther;
	private Vector3 getTapThis;
	private Vector3 tapOther;
	private Vector3 tapThis;

	private bool getTap = false;
	private bool tap = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 GetTap (Vector3 otherPos) {
		Vector3 myPos = transform.position;
		getTapOther = otherPos;
		getTapThis = myPos;
		getTap = true;

		if (tap) {
		//	InitOrigin ();
		} else {
			getTap = true;
		}

		return getTapThis;
	}

	public void Tap(string otherID, Vector3 tapThis) {
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();
		if (!otherTC.isLocalPlayer) {
				return;
		}

		this.tapThis = tapThis;
		tapOther = otherTC.GetTap(tapThis);

		RpcRemoteTap (otherID, tapThis, tapOther);
	}

	[ClientRpc]
	public void RpcRemoteTap(string otherID, Vector3 tapThis, Vector3 tapOther){
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();
		if (!otherTC.isLocalPlayer) {
			return;
		}

		this.tapThis = tapThis;
		this.tapOther = tapOther;

		if (getTap) {
		//	InitOrigin ();
		} else {
			tap = true;
		}
	}

	private void InitOrigin(){

		//TODO: assume same height
		getTapOther.y = 0;
		getTapThis.y = 0;
		tapOther.y = 0;
		tapThis.y = 0;

		Vector3 otherVector = getTapOther - tapOther;
		Vector3 thisVector = getTapThis - tapThis;

		float angleOtherToThis = Vector3.SignedAngle (otherVector, thisVector, Vector3.up);
		Vector3 offsetOtherToThis = getTapThis - Quaternion.AngleAxis (angleOtherToThis, Vector3.up) * getTapOther;

		GameObject thisOrigin = Instantiate(otherOriginPrefab, offsetOtherToThis, Quaternion.Euler(0, angleOtherToThis, 0));
		thisOrigin.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar(name);

		GetComponent<PlayerControl> ().SetGameStarted (thisOrigin);

	}

}
