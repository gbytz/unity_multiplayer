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

	public MapSession mapSession;
	public String DEV_KEY;
	private List<MapAsset> currentAssets = new List<MapAsset> ();

	public GameObject thisOrigin;
	public GameObject otherOriginPrefab;
	public HostSetup thisHostSetup;		

	public void JoinScene (){
		if (thisHostSetup.isServer) {
			StartHost ();
		} else {
			StartClient ();
		}
	}

	private void StartHost ()
	{
		InitHostMappingSession ();
		hostClientPanel.SetActive (false);

		Toast ("Scan the area for 5 seconds to build scene", 5.0f);
		Invoke ("EnterScene", 5.0f);
	}

	private void StartClient ()
	{
		InitClientMappingSession ();
		hostClientPanel.SetActive (false);

		Toast ("Scan the area until scene is found.", 5.0f);
	}

	private void InitHostMappingSession ()
	{

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
			playerCanvas.SetActive (true);
		};

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += AssetLoadedCallback;
	}

	private void InitClientMappingSession ()
	{
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
		mapSession.AssetLoadedEvent += AssetLoadedCallback;
	}

	private void EnterScene ()
	{
		SaveAsset (thisOrigin);
	}

	private void SaveAsset (GameObject asset)
	{
		currentAssets.Add (new MapAsset (asset.name, asset.transform.rotation.y, asset.transform.position));
		mapSession.StorePlacements (currentAssets);
	}


	public void StatusChangedCallback (MapStatus mapStatus)
	{
		Debug.Log ("status updated: " + mapStatus);
	}

	public void ClientAssetStoredCallback (bool stored)
	{
		playerCanvas.SetActive (true);
		Toast ("Joined the Scene", 2.0f);
	}

	public void AssetLoadedCallback (MapAsset mapAsset)
	{
				
		if (IsANewAsset (mapAsset)) {
			currentAssets.Add (mapAsset);

			Vector3 position = new Vector3 (mapAsset.X, mapAsset.Y, mapAsset.Z);
			Quaternion orientation = Quaternion.Euler(0, mapAsset.Orientation, 0);
			GameObject newPhone = Instantiate(otherOriginPrefab, -position, orientation);
			newPhone.GetComponent<OtherPhoneSetup> ().InitPhoneAvatar(mapAsset.AssetId);

			if (mapAsset.AssetId == "1") {
				thisHostSetup.AddSelf (thisOrigin.name, position, Quaternion.Inverse(orientation));
				EnterScene ();
			}

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