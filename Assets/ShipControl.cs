using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

	public GameObject laserPrefab;
	public Transform laserSpawn;

	public void Fire(){
		// Create the Bullet from the Bullet Prefab
		var laser = (GameObject)Instantiate (
			laserPrefab,
			laserSpawn.position,
			laserSpawn.rotation);

		// Add velocity to the bullet
		laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * 6;

		// Destroy the bullet after 2 seconds
		Destroy(laser, 2.0f);
	}
}
