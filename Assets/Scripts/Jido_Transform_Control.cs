using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Jido_Transform_Control : NetworkBehaviour
{
	private GameManager _gameManager;

	private GameObject playerModel;

	//Reference Frame
	private Quaternion rotationRemoteToLocal;
	private Vector3 offsetLocalToRemote;

	private Vector3 getTapLocalFrame;
	private Vector3 getTapRemoteFrame;
	private Vector3 tapLocalFrame;
	private Vector3 tapRemoteFrame;

	private bool getTap = false;
	private bool tap = false;
	private bool initialized = false;
	private float updateThresh = 0.5f;

	private Vector3 updateOffset;
	private Quaternion updateRotation;
	private bool updateTarget;
	private float speed = 2.0f;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update ()
	{
		if (isLocalPlayer) {
			if (Input.touchCount > 0 && _gameManager.lookFor.Count > 0) {

				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
					RaycastHit hit;
					bool hitSomething = Physics.Raycast (ray, out hit, Mathf.Infinity);
					Debug.Log ("Hit " + hitSomething.ToString ());
					if (hitSomething) {
						Debug.Log ("Hit " + hit.collider.name);
					}

					if (hitSomething && hit.collider.GetComponent<DetectedObjectControl> () != null) {
						Debug.Log ("tapped person");
						Tap (_gameManager.lookFor.Dequeue (), hit.collider.transform.position);
						Destroy (hit.collider.gameObject);
					}
				}
			}
		} else {
			if (initialized) {
				if (updateTarget) {
					offsetLocalToRemote = Vector3.Lerp (offsetLocalToRemote, updateOffset, Time.deltaTime * speed);
					rotationRemoteToLocal = Quaternion.Lerp (rotationRemoteToLocal, updateRotation, Time.deltaTime * speed);
				}
				playerModel.transform.position = offsetLocalToRemote + rotationRemoteToLocal * transform.position;
				playerModel.transform.rotation = rotationRemoteToLocal * transform.rotation;
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
			localPlayer.GetComponent<PlayerController> ().SetGameStarted ();
		}

		getTapRemoteReturn = getTapRemoteFrame;
		return true;
	}

	//TODO: For testing purposes in Unity Editor
	public void TestTap ()
	{
		Tap (_gameManager.lookFor.Dequeue (), new Vector3 (0, 0, 1));
	}

	private void Tap (string otherID, Vector3 tapLocalFrame)
	{
		Jido_Transform_Control otherTC = GameObject.Find (otherID).GetComponent<Jido_Transform_Control> ();

		otherTC.GetTap (gameObject, tapLocalFrame, out tapRemoteFrame);

		CmdRemoteTap (otherID, tapLocalFrame, tapRemoteFrame);

		_gameManager.UpdateLookForDisplay ();

	}

	public void AutoTap (string otherID, Vector3 tapLocalFrame)
	{
		Jido_Transform_Control otherTC = GameObject.Find (otherID).GetComponent<Jido_Transform_Control> ();

		if (otherTC.GetTap (gameObject, tapLocalFrame, out tapRemoteFrame)) 
        {
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
		Jido_Transform_Control otherTC = otherPlayer.GetComponent<Jido_Transform_Control> ();
		if (!otherTC.isLocalPlayer) {
			return;
		}

		this.tapRemoteFrame = tapRemote;
		this.tapLocalFrame = tapLocal;
		tap = true;

		print ("Got Remote Tapped");

		if (getTap) {
			InitOrigin ();
			otherPlayer.GetComponent<PlayerController> ().SetGameStarted ();
		} else {
			tap = true;
		} 
	}

	private void InitOrigin ()
	{
		Vector3 localVector = getTapLocalFrame - tapLocalFrame;
		Vector3 remoteVector = getTapRemoteFrame - tapRemoteFrame;

		float angleRemoteToLocal = Vector3.SignedAngle (remoteVector, localVector, Vector3.up);
		Quaternion _rotationRemoteToLocal = Quaternion.Euler (0, angleRemoteToLocal, 0);
		Vector3 offsetLocalToRemoteA = getTapLocalFrame - rotationRemoteToLocal * getTapRemoteFrame;
		Vector3 offsetLocalToRemoteB = tapLocalFrame - rotationRemoteToLocal * tapRemoteFrame;
		Vector3 _offsetLocalToRemote = (offsetLocalToRemoteA + offsetLocalToRemoteB) / 2;

		if (!initialized) {
			offsetLocalToRemote = _offsetLocalToRemote;
			rotationRemoteToLocal = _rotationRemoteToLocal;
			playerModel = Instantiate (_gameManager.PlayerModelPrefab);
			playerModel.name = name;
			GetComponent<PlayerController>().SetGameStarted (playerModel);
			initialized = true;
		} else {
			updateTarget = true;
			updateOffset = _offsetLocalToRemote;
			updateRotation = _rotationRemoteToLocal;
		}
	}

	public Vector3 GetLocalPosition(Vector3 remotePosition)
    {
		return offsetLocalToRemote + rotationRemoteToLocal * remotePosition;
	}

	private Vector3 ConvertPhoneToHumanCentroid(Vector3 phonePos)
    {
		Vector3 offsetZ = transform.TransformPoint(new Vector3 (0f, 0f, -.1f));
		Vector3 offsetY = new Vector3 (0f, -.2f, 0f);

		return offsetZ + offsetY;
	}
}
