//Main control script for players. Handles scene starting and shooting
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public string playerID;
    public GameManager _gameManager;

    //[SyncVar(hook = "OnChangeHealth")]
    public int Health;
    private Image _localHealthBar;
    public const int MaxHealth = 100;
    public int CurrentHealth = MaxHealth;

	//Ship used for Local player
	public GameObject LocalPlayerObject;
    public GameObject ShipVisualsParent;
    public GameObject ShieldVisuals;

    [Header("Projectile Info")]
    public GameObject ProjectilePrefab;
    public Transform ProjectileSpawnPoint;
    private float _maxSpeed = 6;

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

        if (!isLocalPlayer)
        {
            _gameManager.AddNonLocalPlayer(gameObject);
            ShipVisualsParent.SetActive(true);
        }
        else
        {
            //Attach to camera and zero out the rotation in case it's changed
            transform.SetParent(Camera.main.transform, false);
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);

            Health = MaxHealth;

            //Set the name of this player game object using netId
            playerID = GetComponent<NetworkIdentity>().netId.ToString();
            name = playerID;

            _localHealthBar = _gameManager.LocalPlayerHealthBar;

            _gameManager.ShieldButton.onClick.AddListener(ActivateShield);
            _gameManager.AddLocalPlayer(gameObject);
            ShipVisualsParent.SetActive(false);
        }

    }

    void Update () {

		//Only shoot if local player
		if (!isLocalPlayer) {
            //Taken from other phone set up
            //transform.position = transform.position + transform.rotation * phoneGO.transform.position;
            //transform.rotation = transform.rotation * phoneGO.transform.rotation;
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
                Fire (speedFraction);
				CmdFire (speedFraction);
				count = 1;
			}
        }
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

    private void ActivateShield (){
        if (isLocalPlayer)
        {
            ShieldVisuals.SetActive(true);
            CmdActivateShield();
        }
    }

    private void DeactivateShield (){

        ShieldVisuals.SetActive(false);

    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdHit();
    }

    private void Fire(float speedFraction)
    {
        if (!isLocalPlayer)
            return;

        // Create the Bullet from the Bullet Prefab
        var laser = (GameObject)Instantiate(
            ProjectilePrefab,
            ProjectileSpawnPoint.position,
            ProjectileSpawnPoint.rotation);

        // Add velocity to the bullet scaled by how long user touched down
        laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * _maxSpeed * speedFraction;

        // Destroy the bullet after 4 seconds
        Destroy(laser, 4.0f);
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

        print("Remote Fire Before Game Started");

		if (!GameStarted) {
			return;
		}
         
        print("Remote Fire After Game Started");

        //Pass Fire to the remote player's local avatar
        Fire(speedFraction);
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

        _localHealthBar.fillAmount = MaxHealth / 100;
    }

    [Command]
    void CmdActivateShield()
    {
        RpcActivateShield();

    }
    //Send Fire to Client players
    [ClientRpc]
    void RpcActivateShield()
    {
        //For some reason this sometimes get called on the local player
        ActivateShield();
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
