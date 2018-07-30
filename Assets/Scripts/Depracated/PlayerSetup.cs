//Sets up players when they enter the scene
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerSetup : NetworkBehaviour {

	public string playerID;
	public Jido_Manager _jidoManager;


	void Start () {

		//Set the name of this player game object using netId
		playerID = GetComponent<NetworkIdentity>().netId.ToString();
		name = playerID;

        //Add this player game object to the list of players in SceneControl
		_jidoManager = FindObjectOfType<Jido_Manager>();
		if (!isLocalPlayer) {
			_jidoManager.AddNonLocalPlayer (gameObject);
		} else {
			_jidoManager.AddLocalPlayer (gameObject);
		}

	}		
		
}
