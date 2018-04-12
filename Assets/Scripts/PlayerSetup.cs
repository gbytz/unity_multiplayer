using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

//	[SyncVar(hook = "SetPlayerID")]
	public string playerID;
//	private bool initialized;

	void Start () {

		playerID = GetComponent<NetworkIdentity>().netId.ToString();

		name = playerID;

		if (!isLocalPlayer) {
			return;
		}

		GameObject.Find ("This Origin").name = playerID;
	}
		
		
}
