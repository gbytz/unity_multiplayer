using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public bool ShouldBillBoard = true;
    public OtherPlayerController Controller;
    public RectTransform HealthBar;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if(ShouldBillBoard)
            transform.LookAt(Camera.main.transform);
	}
}
