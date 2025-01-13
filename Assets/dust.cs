using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class dust : MonoBehaviour
{
    public float forceApp;
    public int damage;

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 9) {
            other.GetComponentInParent<Rigidbody>().AddForce(new Vector3(0, forceApp, 0));
            other.GetComponentInParent<PlayerMovement>().health -= damage;
        }
    }
}
