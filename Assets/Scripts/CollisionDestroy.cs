using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroy : MonoBehaviour {

    public bool IsLocalPlayer;
    public string LocalShieldTag;
    public string ClientShieldTag;

	// Use this for initialization
	void OnTriggerEnter(Collider collider){

        if(IsLocalPlayer && collider.tag == ClientShieldTag)
        {
            Destroy(gameObject);
        }
        else if(!IsLocalPlayer && collider.tag == LocalShieldTag)
        {
            Destroy(gameObject);
        }
	}
}
