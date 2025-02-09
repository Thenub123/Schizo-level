using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    public int health;

    public Material hit_mat;
    public Material default_mat;

    private IEnumerator hitCoroutine;

    public bool boss;

    public Animator bossAnim;
    public bool bossEnabled;

    public TMP_Text healthText;
    public Slider slider;

    public int bossTimer;
    public bool canAttack;
    public bool canDrop;
    public bool isDead;

    public GameObject drop;
    public Transform player;
    public PlayerMovement playerMovement;

    void Update()
    {
        if(health <= 0){
            Die();
        }

        if(boss) {
            healthText.text = health.ToString();
            slider.value = health;
        }

        if(bossEnabled) {
            bossAnim.SetBool("Enabled", true);
        }

        if(canAttack && bossEnabled && !isDead) {
            canAttack = false;
            IEnumerator animTimer = AttackTimer();
            StartCoroutine(animTimer);
        }
    }

    private void Die() {
        if(bossEnabled) {
            bossAnim.SetBool("Dead", true);
            isDead = true;
        } else {
            Destroy(gameObject);
        }
        
    }

    public void Hit() {
        hitCoroutine = HitTimer(0.1f);
        StartCoroutine(hitCoroutine);
    }



    private IEnumerator HitTimer(float waitTime)
    {
        gameObject.GetComponent<MeshRenderer>().material = hit_mat;
        yield return new WaitForSeconds(waitTime);
        gameObject.GetComponent<MeshRenderer>().material = default_mat;
    }

    private IEnumerator AttackTimer()
    {
        int num = Random.Range(0, 3);
        if(num == 0) {
            bossAnim.SetTrigger("Right");
        }
        else if(num == 1) {
            bossAnim.SetTrigger("Left");
            GameObject dropped = Instantiate(drop, new Vector3(player.position.x, -13.3f, player.position.z), player.rotation);
            dropped.SetActive(true);
        }
        else {
            bossAnim.SetTrigger("Jump");
        }
        yield return new WaitForSeconds(bossTimer);
        canAttack = true;
    }
}
