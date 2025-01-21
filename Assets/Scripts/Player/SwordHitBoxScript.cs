using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitBoxScript : MonoBehaviour
{
    public PlayerController playerController;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other){
        playerController.OnSwordCollisionEnter(other);
    }
}
