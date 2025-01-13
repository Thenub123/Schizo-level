using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnabled : MonoBehaviour
{

    public Animator arena;
    public Animator healthBar;
    public Enemy boss;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 9) {
            arena.SetBool("Enabled", true);
            healthBar.SetBool("Enabled", true);
            IEnumerator animTimer = BossTimerEnum();
            StartCoroutine(animTimer);
        }
    }

    private IEnumerator BossTimerEnum()
    {
        yield return new WaitForSeconds(3);
        boss.bossEnabled = true;
    }
}
