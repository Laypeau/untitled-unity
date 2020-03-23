using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildExplode : MonoBehaviour
{
    // Excuse the mess, I gave up partway through writing this

    public float delayTime = 0.1f;
    public float delayUntilShrink = 7f;
    public float shrinkTime = 2f;
    public float explosionForce = 50f;

    private Collider myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Terrain"))
        {
            Explode(collision.transform.position);
        }
    }

    /// <summary>
    /// Detaches all children and adds an explosion force to them, based on _HitPos
    /// </summary>
    /// <param name="_HitPos"> Where the ExplosionForce will originate </param>
    public void Explode(Vector3 _HitPos)
    {
        StartCoroutine(DoTheThingAfterALilBit(delayTime, _HitPos));

        myCollider.enabled = false; //Adding a system to delete the gameobject when all coroutines are done is too hard
    }

    /// <summary>
    /// bad
    /// </summary>
    IEnumerator DoTheThingAfterALilBit(float _waitTime, Vector3 _HitPosButAgainBecauseICantOrganiseMyCode)
    {
        yield return new WaitForSeconds(_waitTime);

        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody _ChildRigidbody = transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            _ChildRigidbody.AddExplosionForce(explosionForce, _HitPosButAgainBecauseICantOrganiseMyCode, 0f, 1f, ForceMode.VelocityChange);
            StartCoroutine(ShrinkOverTime(transform.GetChild(i).gameObject, delayUntilShrink, shrinkTime));
        }
    }

    /// <summary>
    /// This coroutine is actuall pretty nifty, but could use some improvement. I'm probably going to forget I ever wrote this and remake it but better which I can't be bothered doing now
    /// </summary>
    IEnumerator ShrinkOverTime(GameObject _gameObject, float _delayTime, float _shrinkTime)
    {
        float _startTime = Time.time;
        while (Time.time < _startTime + _delayTime)
        {
            yield return null;
        }

        _startTime = Time.time;
        while (Time.time < _startTime + _shrinkTime)
        {
            _gameObject.transform.localScale = Vector3.one * (((_startTime + _shrinkTime) - Time.time) / _shrinkTime); //alternatively, just calculate the percentage of the timespan, store that each loop cycle and base this whole coroutine on that

            yield return null;
        }

        Destroy(_gameObject);
    }
}
