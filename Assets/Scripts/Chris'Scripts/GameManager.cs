using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.iOS;

public class GameManager : MonoBehaviour {

    public SceneControl SceneController;

    [Header ("ArKit Objects To Activate")]
    [SerializeField]private UnityARVideo _video = null;
    [SerializeField] private UnityARCameraNearFar _camNearFar = null;
    [SerializeField] private UnityARCameraManager _camManager = null;
    #if UNITY_EDITOR
    [SerializeField] private ARKitRemoteConnection _arRemote = null;//Doesn't build to device
    #endif

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void StartGame ()
    {
        _video.enabled = true;
        _camNearFar.enabled = true;
        _camManager.enabled = true;
        #if UNITY_EDITOR
        _arRemote.enabled = true;
        #endif

        SceneController.enabled = true;

        //StartCoroutine(StartGameRoutine());
    }
}
