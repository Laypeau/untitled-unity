using UnityEngine;
using System;
using PickupItem;

namespace PickupItem
{
    class Pistol : UseItem
    {
        //temp
        private Material defaultMaterial;
        public Material reloadMaterial;

        public int clipSize = 10;
        public int clip = 10;
        public float reloadTime = 2.8f;
        [HideInInspector] public bool reloading = false;
        public float useTime = 0.2f;
        [HideInInspector] private float lastUsed = 0f;

        /// <summary>
        /// Initial velocity of the bullet's rigidbody
        /// </summary>
        public float bulletSpeed = 30f;
        /// <summary>
        /// The offset from the game object's transform, in local space
        /// </summary>
        public Vector3 bulletSpawnOffset = new Vector3(0f, 0f, 0.4f);

        void Start()
        {
            defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            if ((clip <= 0 || Input.GetKeyDown(KeyCode.R)) && GetComponent<PickupItem>().PickedUp && !reloading)
            {
                gameObject.GetComponent<MeshRenderer>().material = reloadMaterial;
                reloading = true;
                Invoke("Reload", reloadTime);
            }
        }

        public override void Use()
        {
            if (clip > 0 && !reloading)
            {
                if (Time.time >= lastUsed + useTime)
                {
                    lastUsed = Time.time;
                    GameObject bullet = ObjectPool.Instance.GetBullet();
                    bullet.GetComponent<Rigidbody>().position = transform.position + (transform.rotation * bulletSpawnOffset);
                    bullet.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * bulletSpeed;
                    clip -= 1;
                }
            }
        }

        public void Reload()
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            reloading = false;
            clip = clipSize;
        }
    }

    interface IProjectileWeapon
    {

    }
}