using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 6){
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == 6){
            isGrounded = false;
        }
    }
}
