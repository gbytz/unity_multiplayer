using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedProjectile : MonoBehaviour {

    public string PlayerTagToHit;
    public LocalPlayerController SentFromPlayer;

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
