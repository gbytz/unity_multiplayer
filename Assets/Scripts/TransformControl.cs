using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TransformControl : NetworkBehaviour
{
	public GameObject tapPrefab;
	public GameObject thisOrigin;
	public float angleRemoteToLocal;
	public Vector3 offsetLocalToRemote;

	public GameObject otherOriginPrefab;
	public SceneControl sceneControl;

	private Vector3 getTapLocalFrame;
	private Vector3 getTapRemoteFrame;
	private Vector3 tapLocalFrame;
	private Vector3 tapRemoteFrame;

	private bool getTap = false;
	private bool tap = false;
	private bool initialized = false;
	private float updateThresh = 0.5f;


    private void Start()
    {
        sceneControl = FindObjectOfType<SceneControl>();
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

	public bool GetTap (GameObject localPlayer, Vector3 localPos, out Vector3 getTapRemoteReturn)
	{
		Debug.Log ("GotTapped");
		Vector3 remotePos = ConvertPhoneToHumanCentroid(transform.position);

		if (initialized) {
			Vector3 remoteDiff = remotePos - getTapRemoteFrame;
			Vector3 localDiff = localPos - getTapLocalFrame;

			if (Vector3.Distance (remoteDiff, localDiff) > updateThresh) {
				print ("Too Far: " + Vector3.Distance (remoteDiff, localDiff));
				getTapRemoteReturn = Vector3.zero;
				return false;
			}

			print ("Close enough: " + Vector3.Distance (remoteDiff, localDiff));

		}

		getTapLocalFrame = localPos;
		getTapRemoteFrame = remotePos;
		getTap = true;

		if (tap) {
			InitOrigin ();
			localPlayer.GetComponent<PlayerControl> ().SetGameStarted ();
		}

		getTapRemoteReturn = getTapRemoteFrame;
		return true;
	}

	//TODO: For testing purposes in Unity Editor
	public void TestTap ()
	{
		Tap (sceneControl.lookFor.Dequeue (), new Vector3 (0, 0, 1));
	}

	private void Tap (string otherID, Vector3 tapLocalFrame)
	{
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();

		otherTC.GetTap (gameObject, tapLocalFrame, out tapRemoteFrame);

		CmdRemoteTap (otherID, tapLocalFrame, tapRemoteFrame);

		sceneControl.UpdateLookForDisplay ();

	}

	public void AutoTap (string otherID, Vector3 tapLocalFrame)
	{
		TransformControl otherTC = GameObject.Find (otherID).GetComponent<TransformControl> ();

		if (otherTC.GetTap (gameObject, tapLocalFrame, out tapRemoteFrame)) {
			CmdRemoteTap (otherID, tapLocalFrame, tapRemoteFrame);
		}
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
		tap = true;

		if (getTap) {
			InitOrigin ();
			otherPlayer.GetComponent<PlayerControl> ().SetGameStarted ();
		} else {
			tap = true;
		} 
	}


	private void InitOrigin ()
	{

		Vector3 localVector = getTapLocalFrame - tapLocalFrame;
		Vector3 remoteVector = getTapRemoteFrame - tapRemoteFrame;

		angleRemoteToLocal = Vector3.SignedAngle (remoteVector, localVector, Vector3.up);
		Vector3 offsetLocalToRemoteA = getTapLocalFrame - Quaternion.AngleAxis (angleRemoteToLocal, Vector3.up) * getTapRemoteFrame;
		Vector3 offsetLocalToRemoteB = tapLocalFrame - Quaternion.AngleAxis (angleRemoteToLocal, Vector3.up) * tapRemoteFrame;
		offsetLocalToRemote = (offsetLocalToRemoteA + offsetLocalToRemoteB) / 2;

		if (!initialized) {
			thisOrigin = Instantiate (otherOriginPrefab, offsetLocalToRemote, Quaternion.Euler (0, angleRemoteToLocal, 0));
			thisOrigin.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar (name);

			GetComponent<PlayerControl> ().SetGameStarted (thisOrigin);

			initialized = true;
		} else {
			thisOrigin.GetComponent<SmoothUpdate>().SetTarget(offsetLocalToRemote, Quaternion.Euler (0, angleRemoteToLocal, 0));
		}

		print ("Init Origin");

	}

	public Vector3 GetLocalPosition(Vector3 remotePosition){
		return offsetLocalToRemote + Quaternion.AngleAxis (angleRemoteToLocal, Vector3.up) * remotePosition;
	}

	private Vector3 ConvertPhoneToHumanCentroid(Vector3 phonePos){
		Vector3 offsetZ = transform.TransformPoint(new Vector3 (0f, 0f, -.1f));
		Vector3 offsetY = new Vector3 (0f, -.2f, 0f);

		return offsetZ + offsetY;
	}


}
