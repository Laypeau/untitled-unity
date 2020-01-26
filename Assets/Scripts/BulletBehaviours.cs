    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviours : MonoBehaviour
{
    private float BulletForce = 50f;
    private float LifeDuration = 5f;
    private float LifeTimer;

    void OnEnable()
    {
        LifeTimer = LifeDuration;
        //Vector3 ForceToAdd = GameObject.Find("Player").transform.rotation.eulerAngles.normalized * BulletForce;
        //gameObject.GetComponent<Rigidbody>().AddForce(BulletForce * Vector3.forward);
    }

    void Update()
    {
        LifeTimer -= Time.deltaTime;
        if (LifeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
