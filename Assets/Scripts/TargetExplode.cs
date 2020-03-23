using System.Collections;
using UnityEngine;

public class TargetExplode : MonoBehaviour
{
    public float explosionForce = 8f;
    private MenuManagement.MenuManager Menu;

    void Start()
    {
        Menu = GameObject.Find("Canvas").GetComponent<MenuManagement.MenuManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Terrain"))
        {
            Explode(collision.transform.position);
        }
    }

    /// <summary>
    /// Detaches all children and adds an explosion force to them, based on _HitPos, with a radius of _ExplosionRadius
    /// </summary>
    /// <param name="_HitPos"> Where the ExplosionForce will originate </param>
    /// <param name="_ExplosionRadius"> The radius of the explosion </param>
    public void Explode(Vector3 _HitPos)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody _ChildRigidbody = transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            _ChildRigidbody.AddExplosionForce(explosionForce, _HitPos, 0f, 1f, ForceMode.VelocityChange);
            StartCoroutine(ShrinkAway(transform.GetChild(i).gameObject, 3f, 2f));
        }
        transform.DetachChildren();

        Menu.GetComponent<MenuManagement.MenuManager>().UpdateScore(1);
        Destroy(gameObject);
    }

    IEnumerator ShrinkAway(GameObject _gameObject, float _delayTime, float _shrinkTime)
    {
        Vector3 _startingScale = transform.localScale;

        float _startTime = Time.unscaledTime;
        while (_startTime + _delayTime > Time.unscaledTime)
        {
            yield return null;
        }

        _startTime = Time.unscaledTime;
        while (_startTime + _shrinkTime > Time.unscaledTime)
        {
            _gameObject.transform.localScale = _startingScale * (1 - (((_startTime + _shrinkTime) - Time.unscaledTime) / _shrinkTime));
        }

        Destroy(gameObject);
    }
}
