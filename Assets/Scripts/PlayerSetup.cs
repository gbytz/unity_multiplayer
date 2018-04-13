using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	public string playerID;

	void Start () {

		playerID = GetComponent<NetworkIdentity>().netId.ToString();
		name = playerID;

		if (!isLocalPlayer) {
			return;
		}

		GameObject.Find ("This Origin").name = playerID;
	}

	public override void OnStartLocalPlayer()
	{
		GameObject.Find ("GUI").GetComponent<SceneControl> ().Toast ("When ready, press JOIN button", 4.0f);
	}
		
		
}
