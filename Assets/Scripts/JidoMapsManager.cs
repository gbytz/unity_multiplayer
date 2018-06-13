using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JidoMapsManager : MonoBehaviour {

	public GameManager _gameManager;

	[Header("Jido Maps Objects")]
	//JidoMaps Stuff
	private MapSession _mapSessionInstance;

	//Game Level Stuff
	private float _detectThreshold = 0.7f;

	void Start(){

		_gameManager = FindObjectOfType<GameManager>();

		_mapSessionInstance = FindObjectOfType<MapSession>();

		//This should be added to the network join process
		//i.e. Join/Host Match, Start Game...Start Map Session
		Invoke ("InitMappingSession", 2.0f);
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

	public void ObjectDetectedCallback(DetectedObject detectedObject){

		if (detectedObject.Confidence > _detectThreshold) 
		{
			_gameManager.AddDetectedObject (detectedObject);
		}
	}

	public void StatusChangedCallback (MapStatus mapStatus)
	{
		Debug.Log ("status updated: " + mapStatus);
	}

	public void AssetStoredCallback (bool stored){}

	public void AssetLoadedCallback (MapAsset mapAsset){}

}
