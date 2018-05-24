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
	public GameObject detectedObjectPrefab;

	private float detectThresh = 0.7f;

	private GameObject localPlayer;
	public Queue<string> lookFor = new Queue<string>();
	public Text lookForText;

	//TODO: Just for testing in editor. Simulates tapping a detected person object
	public void TestTap(){
		localPlayer.GetComponent<TransformControl>().TestTap ();
	}

	void Start(){
		//Wait 2 seconds for ARKit to start before starting Jido mapSession
		//TODO: better way to ensure Jido mapSession starts after ARKit kicks off
		Invoke ("InitMappingSession", 2.0f);
	}

	private void InitMappingSession ()
	{

		//Mapsession initialization parameters
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		//Initialize mapSession
		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		//Set callback to handle when objects are detected
		mapSession.ObjectDetectedEvent += ObjectDetectedCallback;

		//Set callback to handle MapStatus updates
		mapSession.StatusChangedEvent += mapStatus => {};

		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += stored => {};

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += mapAsset => {};

	}

	public void ObjectDetectedCallback(DetectedObject detectedObject){
		if (detectedObject.Confidence > detectThresh) {

			if (lookFor.Count > 0 && detectedObject.Name == "person") {
				Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
				GameObject SUPlayer = Instantiate (SUPlayerPrefab, pos, Quaternion.identity);
				SUPlayer.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
			} else if (lookFor.Count < 1 && detectedObject.Name == "chair"){
				Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
				GameObject DO = Instantiate (detectedObjectPrefab, pos, Quaternion.identity);
				DO.transform.localScale = new Vector3 (detectedObject.Height / 2, detectedObject.Height, detectedObject.Height / 2);
			}
		}
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