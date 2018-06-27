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

        healthBar = FindObjectOfType<GameManager>().LocalPlayerHealthBar;
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
            FindObjectOfType<GameManager>().ShowToast("Dead!", 4.0f);
		}

        healthBar.fillAmount = maxHealth / 100;
		//healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
	}
}