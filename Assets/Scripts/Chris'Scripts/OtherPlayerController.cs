using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour {

    //Player Stuff
    public GameObject playerCanvas;
    public RectTransform myHealth;
    public GameObject thisOrigin;


    //SpaceShip Example Specific
    public RectTransform HealthBar; //This ship's healthBar display
    public GameObject ProjectilePrefab;
    public Transform SpawnPoint;

    private float maxSpeed = 6;

    public int Health = 100;
    public Renderer PlayerMeshRenderer;
    private Material _playerMaterial;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
