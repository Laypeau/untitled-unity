using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody bulletRigidbody;
    LineRenderer lineRenderer;

    public float minVelocity = 3f;
    public float lifeTime = 5f;
    private float currentLife;

    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        currentLife = lifeTime;
    }

    void Update()
    {
        if (currentLife > 0f)
        {
            currentLife -= Time.deltaTime;
        }

        if(currentLife <= 0f || bulletRigidbody.velocity.magnitude < minVelocity)
        {
            gameObject.SetActive(false);
        }
    }
}
