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

	private void InitMappingSession ()
	{

		//Mapsession initialization
		bool isMappingMode = true;
		string mapID = "Jido";
		string userID = "Multiplayer";

		mapSession.Init (isMappingMode ? MapMode.MapModeMapping : MapMode.MapModeLocalization, userID, mapID);

		//Set callback to handly MapStatus updates
		mapSession.StatusChangedEvent += StatusChangedCallback;

		//Set callback that confirms when assets are stored
		mapSession.AssetStoredEvent += AssetStoredCallback;

		//Set Callback for when assets are reloaded
		mapSession.AssetLoadedEvent += AssetLoadedCallback;

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