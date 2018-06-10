//Main control script for players. Handles scene starting and shooting
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalPlayerController : NetworkBehaviour {

    public string playerID;
    public GameManager _gameManager;

    public int Health;
    public Image HealthBarImage;
    public const int MaxHealth = 100;
    public int CurrentHealth = MaxHealth;


	//Ship used for Local player
	public GameObject LocalPlayerObject;
    public GameObject ShopModelParent;


	//Origin of this player's refrence frame
	public GameObject thisOrigin;

	//Flag set in SetGameStarted method and accessed by SceneControl
	public bool GameStarted;

	//Variables to track how long user has been touching for a shoot
	private float maxCount = 30f;
	private int count = 1;

    private Transform _cameraTransform;


    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _gameManager = FindObjectOfType<GameManager>();
        HealthBarImage = _gameManager.LocalPlayerHealthBar;

        //Set the name of this player game object using netId
        playerID = GetComponent<NetworkIdentity>().netId.ToString();
        name = playerID;

        if (!isLocalPlayer)
        {
            _gameManager.AddNonLocalPlayer(gameObject);
            ShopModelParent.SetActive(true);
        }
        else
        {
            _gameManager.AddLocalPlayer(gameObject);
        }

    }

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


        transform.position = _cameraTransform.position;
        transform.rotation = _cameraTransform.rotation;
	
	}

	//For local player. Set by TransformControl
	public void SetGameStarted(){
		if (!GameStarted) {
			GameStarted = true;
            FindObjectOfType<GameManager>().StartGame();
		}
	}

	//For remote player. Set by OtherPhoneSetup
	public void SetGameStarted(GameObject origin){
		if (!GameStarted) {
			thisOrigin = origin;
   			GameStarted = true;
            FindObjectOfType<GameManager>().StartGame ();
		}
	}

	//Firing for local player happens from a localship that is not visualized
	private void LocalFire(float speedFraction){
		if (GameStarted) {
			LocalPlayerObject.GetComponent<ShipControl> ().Fire (speedFraction);
		}
	}

    private void ActivateShield (){


    }

    private void DeactivateShield (){



    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdHit();
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

		if (!GameStarted) {
			return;
		}

		//Pass Fire to the remote player's local avatar
		thisOrigin.GetComponent<AvatarControl>().Fire (speedFraction);
	}

    [Command]
    void CmdHit()
    {
        RpcChangeHealth();
    }

    [ClientRpc]
    void RpcChangeHealth()
    {
        CurrentHealth -= 10;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = MaxHealth;
            FindObjectOfType<GameManager>().Toast("Dead!", 4.0f);
        }

        HealthBarImage.fillAmount = MaxHealth / 100;
    }

    [Command]
    public void CmdPlaceDetectedObject(Vector3 position)
    {
        RpcPlaceDetectedObject(position);
    }

    [ClientRpc]
    public void RpcPlaceDetectedObject(Vector3 position)
    {
        if (isLocalPlayer)
        {
            return;
        }

        //FindObjectOfType<LocalPlayerController>().ActivateShield();

        //Instantiate(defensePrefab, transformControl.GetLocalPosition(position), Quaternion.identity);
    }
}
