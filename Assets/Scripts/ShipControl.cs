//Controls the ship (local or remote)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipControl : MonoBehaviour {

    public GameObject ProjectilePrefab;
	public Transform SpawnPoint;
	public Image HealthBar; //This ship's healthBar display

	private float maxSpeed = 6;

    public int Health = 100;
    public Renderer PlayerMeshRenderer;
    private Material _playerMaterial;


    private void Start()
    {
        _playerMaterial = PlayerMeshRenderer.material;

        Invoke("TakeDamage", 2);
    }


    public void Fire(float speedFraction){
		// Create the Bullet from the Bullet Prefab
		var laser = (GameObject)Instantiate (
			ProjectilePrefab,
			SpawnPoint.position,
			SpawnPoint.rotation);

		// Add velocity to the bullet scaled by how long user touched down
		laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * maxSpeed * speedFraction;
	
		// Destroy the bullet after 4 seconds
		Destroy(laser, 4.0f);
	}


    public void TakeDamage()
    {
        _playerMaterial.SetColor("_EmissionColor", Color.red);
        Debug.Log("Changed to Red!");

    }
}
