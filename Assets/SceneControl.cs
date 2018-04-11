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
	public GameObject saveButton;
	public GameObject enterButton;

	public Text notification;

	public GameObject mapSessionPrefab;
	public String DEV_KEY;
	private MapSession mapSession;
	private List<MapAsset> currentAssets = new List<MapAsset> ();

	public GameObject thisOrigin;
	public GameObject mapInitPrefab;
	public GameObject phoneLocationPrefab;

	void Start ()
	{

	}

	void Update ()
	{

	}

	public void StartHost ()
	{
		InitHostMappingSession ();
		saveButton.SetActive (true);
		hostClientPanel.SetActive (false);
	}

	public void StartClient ()
	{
		InitClientMappingSession ();
		enterButton.SetActive (true);
		enterButton.SetActive (false);
	}

	public void SaveScene ()
	{
		saveButton.SetActive (false);
		GameObject mapInit = Instantiate (mapInitPrefab);
		SaveAsset (mapInit);
		Toast ("Saving Scene", 3);
	}

	public void EnterScene ()
	{
		enterButton.SetActive (false);
		SaveAsset (thisOrigin);
	}

	private void InitHostMappingSession ()
	{

		mapSession = Instantiate (mapSessionPrefab).GetComponent<MapSession>();

		//Mapsession initialization
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += stored => {
			Toast ("Scene Saved", 2.0f);
			SwitchMappingToLocalization ();
		};

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += mapAsset => {
		};
	}

	public void InitClientMappingSession ()
	{
		mapSession = Instantiate (mapSessionPrefab).GetComponent<MapSession>();

		//Mapsession initialization
		bool isMappingMode = false;
		string mapID = "Jido";
		string userID = "Multiplayer";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += ClientAssetStoredCallback;

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += ClientAssetLoadedCallback;
	}

	private void SaveAsset (GameObject asset)
	{
		currentAssets.Add (new MapAsset (asset.name, asset.transform.rotation.y, asset.transform.position));
		mapSession.StorePlacements (currentAssets);
	}

	private void SwitchMappingToLocalization ()
	{
		MapSession temp = mapSession;
		mapSession = null;
		Destroy (temp);

		InitClientMappingSession ();
		enterButton.SetActive (true);
	}

	public void StatusChangedCallback (MapStatus mapStatus)
	{
		Debug.Log ("status updated: " + mapStatus);
		Toast (mapStatus.ToString (), 2.0f);
	}

	public void ClientAssetStoredCallback (bool stored)
	{
		Toast ("Added Self to Scene", 2.0f);
	}

	public void ClientAssetLoadedCallback (MapAsset mapAsset)
	{
		if (mapAsset.AssetId == "MapInit") {
			return;
		}
				
		if (IsANewAsset (mapAsset)) {
			currentAssets.Add (mapAsset);

			Vector3 position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
			GameObject instantiatedAsset = Instantiate(phoneLocationPrefab, position, orientation);

			Toast("Welcome new player to scene", 2.0f);
		}

	}

	private bool IsANewAsset (MapAsset asset)
	{
		foreach (var loaded in currentAssets) {
			if (asset.AssetId == loaded.AssetId) {
				return false;
			}
		}

		return true;
	}

	private void Toast (String message, float time)
	{
		notification.text = message;
		notification.gameObject.SetActive (true);
		CancelInvoke ();
		Invoke ("ToastOff", time);
	}

	private void ToastOff ()
	{
		notification.gameObject.SetActive (false);
	}
}