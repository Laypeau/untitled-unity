using System.Collections;
using UnityEngine;

namespace PickupItem
{
    public class Bomb : UseItem
    {
        public float explodeRadius = 7f;
        public float explodeForce = 30f;
        public float waitTime = 2f;
        public Vector3 throwForce = new Vector3(0f, 2f, 5f);

        private LayerMask explodeMask;

        void Start()
        {
            explodeMask = ~LayerMask.GetMask("Terrain");
        }

        public override void Use()
        {
            PlayerTransform.GetComponent<PlayerControl.PlayerPickupUse>().PickupPutdown();
            PickupItem.itemRigidbody.AddForce((CameraFocusTransform.rotation *  throwForce), ForceMode.VelocityChange);
            StartCoroutine(CountDown());
        }

        IEnumerator CountDown()
        {
            yield return new WaitForSeconds(waitTime);
            Explode();
            Destroy(gameObject);
        }

        /// <summary>
        /// Adds an explosion force to all detected rigidbodies within an OverlapSphere.
        /// If any collider tagged as "Target" is detected, it's TargetExplode.Explode() function is called
        /// </summary>
        public void Explode()
        {
            Collider[] _ColliderList = Physics.OverlapSphere(transform.position, explodeRadius, explodeMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < _ColliderList.Length; i++)
            {
                if (_ColliderList[i].TryGetComponent(out Rigidbody _rigidbody))
                {
                    _rigidbody.AddExplosionForce(explodeForce, transform.position, explodeRadius, 1f, ForceMode.Impulse);
                }
                else if(_ColliderList[i].CompareTag("Target"))
                {
                    _ColliderList[i].GetComponent<TargetExplode>().Explode(transform.position);
                }
            }
        }
    }
}