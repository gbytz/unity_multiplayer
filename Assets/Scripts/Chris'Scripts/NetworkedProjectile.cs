using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedProjectile : MonoBehaviour {

    public string PlayerTagToHit;
    public PlayerController SentFromPlayer;

    private void HitTarget(){



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == PlayerTagToHit)
        {
            HitTarget();
        }
    }
}
