//Controls the remote player's local avatar. 
//TODO: This file is probably not necessary
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {

	public GameObject thisAvatar;

	//Pass Fire to ShipControl
	public void Fire(float speedFraction){
		thisAvatar.GetComponent<ShipControl>().Fire (speedFraction);
	}
}
