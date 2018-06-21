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
    [Header("Game Level Settings")]
    public GameObject ScannedObjectBoundsPrefab;
    public GameObject PlayerModelPrefab;

    [Header("UI Objects")]
    //UI Controller Stuff
    public Text NotificationText;
    public Queue<string> lookFor = new Queue<string>();
    public Text LookForText;
    public GameObject LookForTextObject;
    public Button ShieldButton;
    public Image LocalPlayerHealthBar;
    public Image ShootChargeRing;

    [Header("PlayerObjects")]
    //Player Stuff
    public GameObject LocalPlayerReference;
    public GameObject InGameUI;

    [Header("PlayerNetworkInfo")]
    public string LocalPlayerID;
    public string OtherPlayerID;

    //ARKit Objects Used to Initialize Manually On Game Start
    [Header("ArKit Objects To Activate")]
    [SerializeField] private UnityARVideo _video = null;
    [SerializeField] private UnityARCameraNearFar _camNearFar = null;
    [SerializeField] private UnityARCameraManager _camManager = null;

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
	}

	public void AddDetectedObject(DetectedObject detectedObject){
		if (detectedObject.Name == "person") {
			
			if (lookFor.Count > 0) {
				GameObject DetectedObjVisual = Instantiate (ScannedObjectBoundsPrefab, detectedObject.Position, Quaternion.identity);
				DetectedObjVisual.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
				DetectedObjVisual.GetComponent<DetectedObjectControl> ().isVisible = true;
			} else {
				GameObject DetectedObjVisual = Instantiate (ScannedObjectBoundsPrefab, detectedObject.Position, Quaternion.identity);
				DetectedObjVisual.transform.localScale = new Vector3 (detectedObject.Height / 3, detectedObject.Height, detectedObject.Height / 3);
				DetectedObjVisual.GetComponent<DetectedObjectControl> ().isVisible = false;
			}
		}
//		} else if (lookFor.Count < 1 && detectedObject.Name == "chair"){
//			GameObject DO = Instantiate (ScannedObjectBoundsPrefab, detectedObject.Position, Quaternion.identity);
//			DO.transform.localScale = new Vector3 (detectedObject.Height / 2, detectedObject.Height, detectedObject.Height / 2);
//		}
	}
		
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
        LocalPlayerReference.GetComponent<Jido_Transform_Control>().TestTap();
    }
}
