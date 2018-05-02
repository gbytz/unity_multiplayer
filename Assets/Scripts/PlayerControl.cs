using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour {

	public GameObject localShip;

	public GameObject thisOrigin;
	private bool gameStarted;

	// Update is called once per frame
	void Update () {

		if (!isLocalPlayer) {
			return;
		}

		if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space))
		{
			LocalFire();
			CmdFire();
		}
	}

	public void SetGameStarted(GameObject origin){
		thisOrigin = origin;
		GetComponent<Health> ().healthBar = thisOrigin.GetComponent<AvatarControl> ().thisAvatar.GetComponent<ShipControl> ().healthBar;
		gameStarted = true;
	}

	private void LocalFire(){
		localShip.GetComponent<ShipControl> ().Fire ();
	}

	[Command]
	void CmdFire(){
		RpcRemoteFire ();
	}

	[ClientRpc]
	void RpcRemoteFire(){
		print ("RPC");
		if (!gameStarted) {
			print ("Not Started");
			return;
		}
		thisOrigin.GetComponent<AvatarControl>().Fire ();
	}
}
