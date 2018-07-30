using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jido_Manager : MonoBehaviour {

	public GameManager _gameManager;

	[Header("Game Prefabs")]
	public GameObject ScannedObjectBoundsPrefab;
	public GameObject PlayerModelPrefab;

	[HideInInspector]
	public GameObject LocalPlayerReference;

	private MapSession _mapSessionInstance;

	public Queue<string> lookFor = new Queue<string>();

	[HideInInspector]
	public string LocalPlayerID;
	[HideInInspector]
	public string OtherPlayerID;

	//Game Level Stuff
	private float _detectThreshold = 0.7f;

	void Start(){

		_gameManager = FindObjectOfType<GameManager>();

		_mapSessionInstance = FindObjectOfType<MapSession>();

		//This should be added to the network join process
		//i.e. Join/Host Match, Start Game...Start Map Session
		Invoke ("InitMappingSession", 2.0f);
	}

	public void AddLocalPlayer(GameObject localPlayer){
		this.LocalPlayerReference = localPlayer;
		LocalPlayerID = localPlayer.name;
	}

	public void AddNonLocalPlayer(GameObject playerID){
		lookFor.Enqueue (playerID.name);
		ShowLookForText ();
		OtherPlayerID = playerID.name;
	}

	public void ShowLookForText ()
	{
		if (lookFor.Count < 1) {
			_gameManager.ToggleToast(false);
		} else {
			_gameManager.ShowToast("Tap Player " + lookFor.Peek() + " When They Light Up!", 0);
		}
	}

	private void InitMappingSession ()
	{
		//Mapsession initialization
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		_mapSessionInstance.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		_mapSessionInstance.ObjectDetectedEvent += ObjectDetectedCallback;
		//Set callback to handly MapStatus updates
		_mapSessionInstance.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		_mapSessionInstance.AssetStoredEvent += AssetStoredCallback;

		//Set Callback for when assets are reloaded
		_mapSessionInstance.AssetLoadedEvent += AssetLoadedCallback;
	}

	public void ObjectDetectedCallback(DetectedObject detectedObject)
	{
		if (detectedObject.Name == "person" && detectedObject.Confidence > _detectThreshold) 
		{
			if (lookFor.Count > 0) {
				GameObject DetectedObjVisual = Instantiate (ScannedObjectBoundsPrefab, detectedObject.Position, Quaternion.identity);
				DetectedObjVisual.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
				DetectedObjVisual.GetComponent<DetectedObjectControl> ().isVisible = true;
			}
			
			Jido_Model[] modelsJUT = FindObjectsOfType<Jido_Model> ();

			foreach (Jido_Model model in modelsJUT) {
				model.UpdateTransform (detectedObject.Position);
			}
		}
	}

	public void TestTap()
	{
		LocalPlayerReference.GetComponent<Jido_Player>().TestTap();
	}

	public void StatusChangedCallback (MapStatus mapStatus)
	{
		Debug.Log ("status updated: " + mapStatus);
	}

	public void AssetStoredCallback (bool stored){}

	public void AssetLoadedCallback (MapAsset mapAsset){}
}
