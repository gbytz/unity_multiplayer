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
    [Header("UI Objects")]
    //UI Controller Stuff
    public Text ToastText;
    public GameObject ToastTextObject;
    public Button ShieldButton;
    public Image LocalPlayerHealthBar;
    public Image ShootChargeRing;
    public GameObject TutorialPage;
    public Button QuitGameBTN;
    public GameObject InGameUI;

    //ARKit Objects Used to Initialize Manually On Game Start
    [Header("ArKit Objects To Activate")]
    [SerializeField] private UnityARVideo _video = null;
    [SerializeField] private UnityARCameraNearFar _camNearFar = null;
    [SerializeField] private UnityARCameraManager _camManager = null;

    private bool _isPaused;

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
}
