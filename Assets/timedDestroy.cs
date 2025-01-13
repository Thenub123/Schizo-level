using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class timedDestroy : MonoBehaviour
{
    public float waitTime;
    void Start()
    {
        IEnumerator destroyTimer = DestroyTimer();
        StartCoroutine(destroyTimer);
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
