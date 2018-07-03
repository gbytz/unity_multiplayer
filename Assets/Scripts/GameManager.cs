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
    public Queue<string> lookFor = new Queue<string>();
    public Text ToastText;
    public GameObject ToastTextObject;
    public Button ShieldButton;
    public Image LocalPlayerHealthBar;
    public Image ShootChargeRing;
    public GameObject TutorialPage;
    public Button QuitGameBTN;

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

    private bool _isPaused;

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
            ToggleToast(false);
		} else {
            ShowToast("Tap Player " + lookFor.Peek() + " When They Light Up!", 0);
		}
	}

	public void StartGame()
    {
        ToggleArKitObjects(true);
		InGameUI.SetActive (true);
	}

    public void PauseGameToggle()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
        }
    }

	public void AddDetectedObject(DetectedObject detectedObject){
		
        if (detectedObject.Name == "person") 
        {	
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
		
	public void ShowToast (String message, float time)
	{
        ToastTextObject.SetActive(true);
        ToastText.text = message;
        ToggleToast(true);

		CancelInvoke ();
        if (time > 0)
        {
            Invoke("ToastOff", time);
        }
	}

	public void ToggleToast (bool OnOff)
	{
        ToastTextObject.SetActive(OnOff);
        ToastText.gameObject.SetActive (OnOff);
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
