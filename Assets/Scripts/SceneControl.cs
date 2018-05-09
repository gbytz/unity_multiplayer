using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{

	public GameObject hostClientPanel;
	public Text notification;
	public GameObject playerCanvas;
	public RectTransform myHealth;

	public MapSession mapSession;
	public String DEV_KEY;

	public GameObject thisOrigin;
	public GameObject otherOriginPrefab;
	public GameObject SUPlayerPrefab;

	private float detectThresh = 0.7f;

	private GameObject localPlayer;
	public Queue<string> lookFor = new Queue<string>();
	public Text lookForText;

	//TODO: remove
	public void TestTap(){
		localPlayer.GetComponent<TransformControl>().TestTap ();
	}

	void Start(){
		Invoke ("InitMappingSession", 2.0f);
	}

	public void AddLocalPlayer(GameObject localPlayer){
		this.localPlayer = localPlayer;
	}

	public void AddNonLocalPlayer(GameObject playerID){
		lookFor.Enqueue (playerID.name);
		UpdateLookForDisplay ();
	}

	public void UpdateLookForDisplay ()
	{
		if (lookFor.Count < 1) {
			lookForText.text = "";
		} else {
			lookForText.text = "Tap player " + lookFor.Peek () + " when they light up!";
		}
	}

	public void StartGame(){
		playerCanvas.SetActive (true);
	}

	private void InitMappingSession ()
	{

		//Mapsession initialization
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		mapSession.ObjectDetectedEvent += ObjectDetectedCallback;
		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += AssetStoredCallback;

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += AssetLoadedCallback;

	}

	public void ObjectDetectedCallback(DetectedObject detectedObject){
		if (lookFor.Count > 0 && detectedObject.Name == "person" && detectedObject.Confidence > detectThresh) {
			Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
			GameObject SUPlayer = Instantiate (SUPlayerPrefab, pos, Quaternion.identity);
			SUPlayer.transform.localScale = new Vector3 (detectedObject.Height / 2, detectedObject.Height / 2, detectedObject.Height / 2);
		}
	}

	public void StatusChangedCallback (MapStatus mapStatus)
	{
		Debug.Log ("status updated: " + mapStatus);
	}

	public void AssetStoredCallback (bool stored){}

	public void AssetLoadedCallback (MapAsset mapAsset){}

	public void Toast (String message, float time)
	{
		notification.text = message;
		notification.gameObject.SetActive (true);
		CancelInvoke ();
		Invoke ("ToastOff", time);
	}

	public void ToastOff ()
	{
		notification.gameObject.SetActive (false);
	}
}