using System.Collections;
using System.Collections.Generic;
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

    public TMP_Text healthText;
    public Slider slider;

    void Update()
    {
        if(health <= 0){
            Die();
        }

        if(boss) {
            healthText.text = health.ToString();
            slider.value = health;
        }
    }

    private void Die() {
        Destroy(gameObject);
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
}
