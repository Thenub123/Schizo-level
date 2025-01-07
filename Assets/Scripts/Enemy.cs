using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health;

    public Material hit_mat;
    public Material default_mat;

    private IEnumerator hitCoroutine;

    void Update()
    {
        if(health <= 0){
            Die();
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
