//Controls the ship (local or remote)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

	public GameObject laserPrefab;
	public Transform laserSpawn;
	public RectTransform healthBar; //This ship's healthBar display

	private float maxSpeed = 6;

	public void Fire(float speedFraction){
		// Create the Bullet from the Bullet Prefab
		var laser = (GameObject)Instantiate (
			laserPrefab,
			laserSpawn.position,
			laserSpawn.rotation);

		// Add velocity to the bullet scaled by how long user touched down
		laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * maxSpeed * speedFraction;
	
		// Destroy the bullet after 4 seconds
		Destroy(laser, 4.0f);
	}
}
