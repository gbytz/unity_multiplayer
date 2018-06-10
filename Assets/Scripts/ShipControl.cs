//Controls the ship (local or remote)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipControl : MonoBehaviour {

    [Header("Prefab References")]
    public GameObject ProjectilePrefab;
	public Transform ProjectileSpawnPoint;

    [Header("Player Properties")]
    public int Health = 100;
    private float maxSpeed = 6;

    [Header("Object References")]
    public Image HealthBar; //This ship's healthBar display
    public Renderer PlayerMeshRenderer;
    //private Material _playerMaterial;


    private void Start()
    {
        //_playerMaterial = PlayerMeshRenderer.material;
        //Invoke("TakeDamage", 2);
    }


    public void Fire(float speedFraction){
		// Create the Bullet from the Bullet Prefab
		var laser = (GameObject)Instantiate (
			ProjectilePrefab,
			ProjectileSpawnPoint.position,
			ProjectileSpawnPoint.rotation);

		// Add velocity to the bullet scaled by how long user touched down
		laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * maxSpeed * speedFraction;
	
		// Destroy the bullet after 4 seconds
		Destroy(laser, 4.0f);
	}


    public void TakeDamage()
    {
        //_playerMaterial.SetColor("_EmissionColor", Color.red);
        Debug.Log("Changed to Red!");
    }
}
