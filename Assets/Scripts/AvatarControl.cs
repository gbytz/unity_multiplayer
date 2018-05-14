using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {

	public GameObject thisAvatar;

	public void Fire(float speedFraction){
		print ("Avatar Fire");
		thisAvatar.GetComponent<ShipControl>().Fire (speedFraction);
	}
}
