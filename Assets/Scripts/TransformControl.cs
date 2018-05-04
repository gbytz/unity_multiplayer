using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TransformControl : NetworkBehaviour
{

	public GameObject otherOriginPrefab;
	public Queue<string> lookFor;
	private Text lookForText;

	private Vector3 getTapOther;
	private Vector3 getTapThis;
	private Vector3 tapOther;
	private Vector3 tapThis;

	private bool getTap = false;
	private bool tap = false;

	void Start (){
		lookForText = GameObject.Find ("LookFor Text").GetComponent<Text> ();
	}
	
	void Update ()
	{

		if (!isLocalPlayer) {
			return;
		}

		if (Input.touchCount > 0 && lookFor.Count > 0) {
			foreach (Touch t in Input.touches) {
				if (Input.GetTouch (t.fingerId).phase == TouchPhase.Began) {
					Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (t.fingerId).position);
					RaycastHit hit;
	
					if (Physics.Raycast (ray, out hit, Mathf.Infinity) && hit.collider.name == "SU Player") {
						Tap (lookFor.Dequeue(), hit.collider.transform.position);
					}
				}
			}
		}
	}
		
	//Only runs on local player
	public void AddLookFor (string playerID)
	{
		lookFor.Enqueue (playerID);
		UpdateLookForDisplay ();
	}

	public Vector3 GetTap (Vector3 otherPos)
	{
		Vector3 myPos = transform.position;
		getTapOther = otherPos;
		getTapThis = myPos;
		getTap = true;

		if (tap) {
			InitOrigin ();
		} else {
			getTap = true;
		}

		return getTapThis;
	}

	private void Tap (string otherID, Vector3 tapThis)
	{
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();

		this.tapThis = tapThis;
		tapOther = otherTC.GetTap (tapThis);

		RpcRemoteTap (otherID, tapThis, tapOther);

		UpdateLookForDisplay ();
	}

	[ClientRpc]
	public void RpcRemoteTap (string otherID, Vector3 tapThis, Vector3 tapOther)
	{
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();
		if (!otherTC.isLocalPlayer) {
			return;
		}

		this.tapThis = tapThis;
		this.tapOther = tapOther;

		if (getTap) {
			InitOrigin ();
		} else {
			tap = true;
		}
	}

	private void InitOrigin ()
	{

		//TODO: assume same height
		getTapOther.y = 0;
		getTapThis.y = 0;
		tapOther.y = 0;
		tapThis.y = 0;

		Vector3 otherVector = getTapOther - tapOther;
		Vector3 thisVector = getTapThis - tapThis;

		float angleOtherToThis = Vector3.SignedAngle (otherVector, thisVector, Vector3.up);
		Vector3 offsetOtherToThis = getTapThis - Quaternion.AngleAxis (angleOtherToThis, Vector3.up) * getTapOther;

		GameObject thisOrigin = Instantiate (otherOriginPrefab, offsetOtherToThis, Quaternion.Euler (0, angleOtherToThis, 0));
		thisOrigin.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar (name);

		GetComponent<PlayerControl> ().SetGameStarted (thisOrigin);

	}

	private void UpdateLookForDisplay ()
	{
		if (lookFor.Count < 1) {
			lookForText.text = "";
		} else {
			lookForText.text = "Tap player " + lookFor.Peek ();
		}
	}

}
