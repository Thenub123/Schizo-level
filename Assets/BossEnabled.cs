using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnabled : MonoBehaviour
{

    public Animator arena;
    public Animator healthBar;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 9) {
            arena.SetBool("Enabled", true);
            healthBar.SetBool("Enabled", true);
        }
    }
}
