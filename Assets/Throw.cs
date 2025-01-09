using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{

    public GameObject pill;

    GameObject pill_thrown;

    public float force;

    public Transform lookat;
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos += Camera.main.transform.forward * -10f;

        Vector3 ray = Camera.main.ScreenToWorldPoint(mousePos);

        lookat.LookAt(ray);

        if(Input.GetMouseButtonDown(0)) {

            pill_thrown = Instantiate(pill, transform.position, transform.rotation);

            pill_thrown.transform.LookAt(ray);
            pill_thrown.GetComponent<Rigidbody>().AddRelativeForce(pill_thrown.transform.right  * force);

        }
    }
}
