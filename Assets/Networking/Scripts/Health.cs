using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;


public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	public int currentHealth = maxHealth;
	public RectTransform healthBar;

	void Start(){
		
		if (!isLocalPlayer) {
			return;
		}

		healthBar = GameObject.Find ("GUI").GetComponent<SceneControl>().myHealth;
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!isLocalPlayer)
		{
			return;
		}

		CmdHit ();

	}

	[Command]
	void CmdHit(){
		RpcChangeHealth ();
	}

	[ClientRpc]
	void RpcChangeHealth ()
	{
		currentHealth -= 1;
		if (currentHealth <= 0)
		{
			currentHealth = maxHealth;
			Debug.Log("Dead!");
		}

		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);

	}
}