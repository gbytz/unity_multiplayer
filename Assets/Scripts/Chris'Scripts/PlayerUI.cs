using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public bool ShouldBillBoard = true;
    public OtherPlayerController Controller;
    public Image HealthBar;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(ShouldBillBoard)
            transform.LookAt(Camera.main.transform);
	}
}
 