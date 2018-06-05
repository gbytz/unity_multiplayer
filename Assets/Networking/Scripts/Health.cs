using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	public int currentHealth = maxHealth;
    public Image healthBar;

	void Start(){
		
		if (!isLocalPlayer) {
			return;
		}

        healthBar = FindObjectOfType<SceneControl>().myHealth;
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
		currentHealth -= 10;
		if (currentHealth <= 0)
		{
			currentHealth = maxHealth;
            FindObjectOfType<SceneControl>().Toast("Dead!", 4.0f);
		}

        healthBar.fillAmount = maxHealth / 100;
		//healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);

	}
}