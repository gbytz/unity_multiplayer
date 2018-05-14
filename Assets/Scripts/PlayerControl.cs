using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour {

	public GameObject localShip;

	public GameObject thisOrigin;
	public bool gameStarted;

	private float maxCount = 80f;
	private int count = 1;

	void Update () {

		if (!isLocalPlayer) {
			return;
		}

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

	public void SetGameStarted(){
		if (!gameStarted) {
			gameStarted = true;
			GameObject.Find ("GUI").GetComponent<SceneControl>().StartGame ();
		}
	}

	public void SetGameStarted(GameObject origin){
		if (!gameStarted) {
			thisOrigin = origin;
			GetComponent<Health> ().healthBar = thisOrigin.GetComponent<AvatarControl> ().thisAvatar.GetComponent<ShipControl> ().healthBar;
			gameStarted = true;
			GameObject.Find ("GUI").GetComponent<SceneControl>().StartGame ();
		}
	}

	private void LocalFire(float speedFraction){
		if (gameStarted) {
			localShip.GetComponent<ShipControl> ().Fire (speedFraction);
		}
	}

	[Command]
	void CmdFire(float speedFraction){
		RpcRemoteFire (speedFraction);
	}

	[ClientRpc]
	void RpcRemoteFire(float speedFraction){
		if (isLocalPlayer) {
			print ("Local RPC");
			return;
		}

		if (!gameStarted) {
			print ("Not Started");
			return;
		}
		thisOrigin.GetComponent<AvatarControl>().Fire (speedFraction);
	}
		
}
