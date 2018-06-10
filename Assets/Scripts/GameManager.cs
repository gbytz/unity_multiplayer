using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    //Game Level Stuff
    private float _detectThreshold = 0.7f;
    [Header("Game Level Settings")]
    public GameObject ScannedObjectBoundsPrefab;
    public GameObject otherOriginPrefab;
    public GameObject detectedObjectPrefab;

    [Header("UI Objects")]
    //UI Controller Stuff
    public Text NotificationText;
    public Queue<string> lookFor = new Queue<string>();
    public Text LookForText;
    public GameObject LookForTextObject;

    [Header("PlayerObjects")]
    //Player Stuff
    public GameObject LocalPlayerReference;
    public GameObject InGameUI;
    public Image LocalPlayerHealthBar;
    public Button ShieldButton;

    [Header("PlayerNetworkInfo")]
    public string LocalPlayerID;
    public string OtherPlayerID;


    [Header("Jido Maps Objects")]
    //JidoMaps Stuff
    private MapSession MapSessionInstance;

    //ARKit Objects Used to Initialize Manually On Game Start
    [Header("ArKit Objects To Activate")]
    [SerializeField] private UnityARVideo _video = null;
    [SerializeField] private UnityARCameraNearFar _camNearFar = null;
    [SerializeField] private UnityARCameraManager _camManager = null;

	void Start(){

        MapSessionInstance = FindObjectOfType<MapSession>();

        //This should be added to the network join process
        //i.e. Join/Host Match, Start Game...Start Map Session
		//Invoke ("InitMappingSession", 2.0f);
	}

    public void LostConnection (){
        InGameUI.SetActive(false);
    }

	public void AddLocalPlayer(GameObject localPlayer){
		this.LocalPlayerReference = localPlayer;
        LocalPlayerID = localPlayer.name;
	}

	public void AddNonLocalPlayer(GameObject playerID){
		lookFor.Enqueue (playerID.name);
		UpdateLookForDisplay ();
        OtherPlayerID = playerID.name;
	}

	public void UpdateLookForDisplay ()
	{
		if (lookFor.Count < 1) {
            LookForText.text = "";
            LookForTextObject.SetActive(false);
		} else {
            LookForText.text = "Tap Player " + lookFor.Peek () + " When They Light Up!";
            LookForTextObject.SetActive(true);
		}
	}

	public void StartGame()
    {
        ToggleArKitObjects(true);
		InGameUI.SetActive (true);
        InitMappingSession();
	}

	private void InitMappingSession ()
	{
		//Mapsession initialization
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		MapSessionInstance.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		MapSessionInstance.ObjectDetectedEvent += ObjectDetectedCallback;
		//Set callback to handly MapStatus updates
		MapSessionInstance.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		MapSessionInstance.AssetStoredEvent += AssetStoredCallback;

		//Set Callback for when assets are reloaded
		MapSessionInstance.AssetLoadedEvent += AssetLoadedCallback;
	}

	public void ObjectDetectedCallback(DetectedObject detectedObject){
		if (detectedObject.Confidence > _detectThreshold) {

			if (detectedObject.Name == "person") {
				if (lookFor.Count > 0) {
					print ("look for: " + lookFor.Count);
					Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
					GameObject DetectedObjVisual = Instantiate (ScannedObjectBoundsPrefab, pos, Quaternion.identity);
                    DetectedObjVisual.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
					DetectedObjVisual.GetComponent<DetectedObjectControl> ().isVisible = true;
				} else {
					Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
					GameObject SUPlayer = Instantiate (ScannedObjectBoundsPrefab, pos, Quaternion.identity);
					SUPlayer.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
					SUPlayer.GetComponent<DetectedObjectControl> ().isVisible = false;
				}
			} else if (lookFor.Count < 1 && detectedObject.Name == "chair"){
					Vector3 pos = new Vector3 (detectedObject.X, detectedObject.Y, -detectedObject.Z);
					GameObject DO = Instantiate (detectedObjectPrefab, pos, Quaternion.identity);
					DO.transform.localScale = new Vector3 (detectedObject.Height / 2, detectedObject.Height, detectedObject.Height / 2);
			}
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
		NotificationText.text = message;
        ToggleToast(true);

		CancelInvoke ();
		Invoke ("ToastOff", time);
	}

	public void ToggleToast (bool OnOff)
	{
        NotificationText.gameObject.SetActive (OnOff);
	}

    void ToggleArKitObjects(bool onOff)
    {
        if (Application.isEditor == false)
        {
            _video.enabled = onOff;
            _camNearFar.enabled = onOff;
            _camManager.enabled = onOff;
        }
    }

    //TODO: remove
    public void TestTap()
    {
        LocalPlayerReference.GetComponent<TransformControl>().TestTap();
    }
}
