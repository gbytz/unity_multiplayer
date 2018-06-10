//Sets up players when they enter the scene
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerSetup : NetworkBehaviour {

	public string playerID;
	public GameManager sceneControl;


	void Start () {

		//Set the name of this player game object using netId
		playerID = GetComponent<NetworkIdentity>().netId.ToString();
		name = playerID;

        //Add this player game object to the list of players in SceneControl
        sceneControl = FindObjectOfType<GameManager>();
		if (!isLocalPlayer) {
			sceneControl.AddNonLocalPlayer (gameObject);
		} else {
			sceneControl.AddLocalPlayer (gameObject);
		}

	}		
		
}
