﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Jido_Player : NetworkBehaviour
{
	public delegate void TransformUpdated();
	public TransformUpdated transformUpdated;

	private GameManager _gameManager;
	private Jido_Manager _jidoManager;

	private GameObject playerModel;

	//Reference Frame
	public Quaternion rotationRemoteToLocal;
	public Vector3 offsetLocalToRemote;

	private Vector3 getTapLocalFrame;
	private Vector3 getTapRemoteFrame;
	private Vector3 tapLocalFrame;
	private Vector3 tapRemoteFrame;

	private bool getTap = false;
	private bool tap = false;
	private bool initialized = false;

	private float errorThresh = 0.25f;
	public float ErrorThresh {
		get{
			return errorThresh;
		}
	}
	private float errorThreshMin = 0.25f;
	private float errorThreshMax = 3f;
	private float seenCount = 0;
	private float seenCountMax = 50;

	private Vector3 updateOffset;
	private Quaternion updateRotation;
	private bool updateTarget;
	private float speed = 2.0f;

	public GameObject dissipateObject;

	private void Start()
	{
		_gameManager = FindObjectOfType<GameManager>();
		_jidoManager = FindObjectOfType<Jido_Manager> ();
	}

	IEnumerator WaitToDestroy(GameObject theObject)
	{
		yield return new WaitForSeconds(0.25f);
		Destroy(theObject);
	}

	void Update ()
	{
		if (isLocalPlayer) {
			if (Input.touchCount > 0 && _jidoManager.lookFor.Count > 0) {
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
					RaycastHit hit;
					bool hitSomething = Physics.Raycast (ray, out hit, Mathf.Infinity);
					if (hitSomething) {
						Debug.Log ("Hit " + hit.collider.name);
					}

					if (hitSomething && hit.collider.GetComponent<DetectedObjectControl> () != null) {
						Tap (_jidoManager.lookFor.Dequeue (), hit.collider.transform.position);
						Destroy (hit.collider.gameObject);
					}
				}
			}
		} else if (initialized) {
			if (updateTarget) {
				offsetLocalToRemote = Vector3.Lerp (offsetLocalToRemote, updateOffset, Time.deltaTime * speed);
				rotationRemoteToLocal = Quaternion.Lerp (rotationRemoteToLocal, updateRotation, Time.deltaTime * speed);
			}

			playerModel.transform.position = offsetLocalToRemote + rotationRemoteToLocal * transform.position;
			playerModel.transform.rotation = rotationRemoteToLocal * transform.rotation;
		}
	}

	public bool GetTap (GameObject localPlayer, Vector3 localPos, out Vector3 getTapRemoteReturn)
	{
		Vector3 remotePos = ConvertPhoneToHumanCentroid(transform.position);

		if (initialized) {
			Vector3 error = localPos - GetLocalPosition (remotePos);

			if (error.magnitude > errorThresh) {
				getTapRemoteReturn = Vector3.zero;
				return false;
			}

			seenCount = 0;

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

	//For testing purposes in Unity Editor
	public void TestTap ()
	{
		Tap (_jidoManager.lookFor.Dequeue (), new Vector3 (0, 0, 1));
	}

	private void Tap (string otherID, Vector3 tapLocalFrame)
	{
		GameObject found = GameObject.Find (otherID);
		Jido_Player otherTC = found.GetComponent<Jido_Player> ();

		otherTC.GetTap (gameObject, tapLocalFrame, out tapRemoteFrame);

		CmdRemoteTap (otherID, tapLocalFrame, tapRemoteFrame);

		_jidoManager.ShowLookForText ();
	}

	public void AutoTap (string otherID, Vector3 tapLocalFrame)
	{
		Jido_Player otherTC = GameObject.Find (otherID).GetComponent<Jido_Player> ();

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
		Jido_Player otherTC = otherPlayer.GetComponent<Jido_Player> ();
		if (!otherTC.isLocalPlayer) {
			return;
		}

		this.tapRemoteFrame = tapRemote;
		this.tapLocalFrame = tapLocal;
		tap = true;

		if (getTap) {
			InitOrigin ();
			otherPlayer.GetComponent<PlayerController> ().SetGameStarted ();
		} 
	}

	public void HitTestPlayerFeet(Vector3 origin, PlayerCallouts playerCallout)
	{
		Ray ray = new Ray(origin, Vector3.down);
		RaycastHit hit;

		//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
		//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
		if (Physics.Raycast(ray, out hit, 30f, (1 << 10)))
		{
			//we're going to get the position from the contact point
			playerCallout.StartTracking(hit.point.y);
			Debug.Log("Hit floor at " + hit.point.y);
			return;
		}

		playerCallout.StartTracking();

	}

	private void InitOrigin ()
	{
		Vector3 localVector = getTapLocalFrame - tapLocalFrame;
		Vector3 remoteVector = getTapRemoteFrame - tapRemoteFrame;

		if (!initialized) {
			float angleRemoteToLocal = Vector3.SignedAngle(remoteVector, localVector, Vector3.up);
			rotationRemoteToLocal = Quaternion.Euler(0, angleRemoteToLocal, 0);
			offsetLocalToRemote = getTapLocalFrame - rotationRemoteToLocal * getTapRemoteFrame;

			playerModel = Instantiate (_jidoManager.PlayerModelPrefab);
			playerModel.name = name;
			HitTestPlayerFeet(playerModel.transform.position, playerModel.GetComponentInChildren<PlayerCallouts>());
			GetComponent<PlayerController>().SetGameStarted (playerModel);
			initialized = true;

			Vector3 offsetLocalToRemoteA = getTapLocalFrame - rotationRemoteToLocal * getTapRemoteFrame;
			Vector3 offsetLocalToRemoteB = tapLocalFrame - rotationRemoteToLocal * tapRemoteFrame;

			offsetLocalToRemote.y = ((offsetLocalToRemoteA + offsetLocalToRemoteB) / 2).y;
		} else {
			Vector3 offsetLocalToRemoteA = getTapLocalFrame - rotationRemoteToLocal * getTapRemoteFrame;
			Vector3 offsetLocalToRemoteB = tapLocalFrame - rotationRemoteToLocal * tapRemoteFrame;

			float updateError = (offsetLocalToRemoteB - offsetLocalToRemoteA).magnitude;
			if (updateError > (errorThresh * 1.2f)) {
				return;
			}

			updateTarget = true;
			updateOffset = getTapLocalFrame - rotationRemoteToLocal * getTapRemoteFrame;
			updateOffset.y = ((offsetLocalToRemoteA + offsetLocalToRemoteB) / 2).y;
		}
	}

	public Vector3 GetLocalPosition(Vector3 remotePosition)
	{
		return offsetLocalToRemote + rotationRemoteToLocal * remotePosition;
	}

	public Quaternion GetLocalRotation(Quaternion remoteRotation)
	{
		return rotationRemoteToLocal * remoteRotation;
	}

	private Vector3 ConvertPhoneToHumanCentroid(Vector3 phonePos)
	{
		Vector3 offsetZ = transform.TransformPoint(new Vector3 (0f, 0f, -.4f));
		Vector3 offsetY = new Vector3 (0f, -.2f, 0f);

		return offsetZ + offsetY;
	}

	public void PlaceSharedObject(string objectPrefab, Vector3 position, Quaternion rotation) 
	{
		if (!isLocalPlayer) {
			return;
		}

		GameObject sharedGO = Instantiate (Resources.Load(objectPrefab, typeof(GameObject)), position, rotation) as GameObject;
		CmdPlaceSharedObject(objectPrefab, position, rotation);
	}

	[Command]
	void CmdPlaceSharedObject(string objectPrefab, Vector3 position, Quaternion rotation){

		RpcPlaceSharedObject(objectPrefab, position, rotation);

	}

	[ClientRpc]
	void RpcPlaceSharedObject(string objectPrefab, Vector3 position, Quaternion rotation){

		if (!initialized) {
			return;
		}

		Vector3 localPosition = GetLocalPosition (position);
		Quaternion localRotation = GetLocalRotation (rotation);

		GameObject sharedGO = Instantiate (Resources.Load(objectPrefab, typeof(GameObject)), localPosition, localRotation) as GameObject;
//		Jido_Shared_Object sharedObjectScript = sharedGO.GetComponent<Jido_Shared_Object> ();
//		if (sharedObjectScript != null && sharedObjectScript.keepUpdated) {
//			sharedObjectScript.initialPosition = localPosition;
//			sharedObjectScript.initialRotation = localRotation;
//			sharedObjectScript.transformControl = this;
//			transformUpdated += sharedObjectScript.OnTransformUpdate;
//		}
	}

	public void ModelWasSeen(){
		if (seenCount > seenCountMax)
		{
			//FindObjectOfType<GameManager>().Toast("Lost Player: " + name, 20f);
			return;
		}

		seenCount++;

		errorThresh = errorThreshMin + errorThreshMax * (seenCount / seenCountMax);

	}

	private void ResetError(){
		seenCount = 0;
		errorThresh = errorThreshMin;
	}
}
