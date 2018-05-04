using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	public string playerID;
	public SceneControl sceneControl;

	void Start () {

		playerID = GetComponent<NetworkIdentity>().netId.ToString();
		name = playerID;
		sceneControl = GameObject.Find ("GUI").GetComponent<SceneControl> ();

		if (!isLocalPlayer) {
			sceneControl.AddNonLocalPlayer (gameObject);
		} else {
			sceneControl.AddLocalPlayer (gameObject);
			GameObject.Find ("This Origin").name = playerID;
		}

	}		
		
}
