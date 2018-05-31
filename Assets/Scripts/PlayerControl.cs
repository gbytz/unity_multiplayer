//Main control script for players. Handles scene starting and shooting
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health))]
public class PlayerControl : NetworkBehaviour {

	//Ship used for Local player
	public GameObject localShip;

	//Origin of this player's refrence frame
	public GameObject thisOrigin;

	//Flag set in SetGameStarted method and accessed by SceneControl
	public bool gameStarted;

	//Variables to track how long user has been touching for a shoot
	private float maxCount = 30f;
	private int count = 1;

	void Update () {

		//Only shoot if local player
		if (!isLocalPlayer) {
			return;
		}

		//Charges shot on touch holding, shoots on touch up
		if (Input.touchCount > 0)  
		{
			if ((Input.GetTouch (0).phase == TouchPhase.Stationary) || (Input.GetTouch (0).phase == TouchPhase.Moved)) {
				if (count < maxCount) {
					count++;
				}
			} else if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				float speedFraction = (float)count / maxCount;
				LocalFire (speedFraction);
				CmdFire (speedFraction);
				count = 1;
			}
		} 
	
	}

	//For local player. Set by TransformControl
	public void SetGameStarted(){
		if (!gameStarted) {
			gameStarted = true;
			GameObject.Find ("GUI").GetComponent<SceneControl>().StartGame ();
		}
	}

	//For remote player. Set by OtherPhoneSetup
	public void SetGameStarted(GameObject origin){
		if (!gameStarted) {
			thisOrigin = origin;
			GetComponent<Health> ().healthBar = thisOrigin.GetComponent<AvatarControl> ().thisAvatar.GetComponent<ShipControl> ().healthBar;
			gameStarted = true;
			GameObject.Find ("GUI").GetComponent<SceneControl>().StartGame ();
		}
	}

	//Firing for local player happens from a localship that is not visualized
	private void LocalFire(float speedFraction){
		if (gameStarted) {
			localShip.GetComponent<ShipControl> ().Fire (speedFraction);
		}
	}

	//Send Fire to Host
	[Command]
	void CmdFire(float speedFraction){
		RpcRemoteFire (speedFraction);
	}

	//Send Fire to Client players
	[ClientRpc]
	void RpcRemoteFire(float speedFraction){
		//For some reason this sometimes get called on the local player
		if (isLocalPlayer) {
			print ("Local RPC");
			return;
		}

		if (!gameStarted) {
			return;
		}

		//Pass Fire to the remote player's local avatar
		thisOrigin.GetComponent<AvatarControl>().Fire (speedFraction);
	}
		
}
