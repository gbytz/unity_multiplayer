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

	void OnStartLocalPlayer(){
		healthBar = GameObject.Find ("My Health").GetComponent<RectTransform>();
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
		currentHealth -= 5;
		if (currentHealth <= 0)
		{
			currentHealth = maxHealth;
			Debug.Log("Dead!");
		}

		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);

	}
}