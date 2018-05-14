using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

	public GameObject laserPrefab;
	public Transform laserSpawn;
	public RectTransform healthBar;

	private float maxSpeed = 6;

	public void Fire(float speedFraction){
		// Create the Bullet from the Bullet Prefab
		var laser = (GameObject)Instantiate (
			laserPrefab,
			laserSpawn.position,
			laserSpawn.rotation);

		// Add velocity to the bullet
		laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * maxSpeed * speedFraction;

		print ("Fire");

		// Destroy the bullet after 2 seconds
		Destroy(laser, 4.0f);
	}
}
