using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    class Shotgun : UseItem
    {
        //temp
        private Material defaultMaterial;
        public Material reloadMaterial;

        public int clipSize = 3;
        public int clip = 3;
        public float reloadTime = 5f;
        [HideInInspector] public bool reloading = false;
        public float useTime = 4f;
        [HideInInspector] private float lastUsed = 0f;

        /// <summary>
        /// Initial velocity of the bullet's rigidbody
        /// </summary>
        public float bulletSpeed = 30f;
        /// <summary>
        /// The offset from the game object's transform, in local space
        /// </summary>
        public Vector3 bulletSpawnOffset = new Vector3(0f, 0f, 0.4f);
        /// <summary>
        /// How many bulets
        /// </summary>
        public int numberOfBullets = 8;
        /// <summary>
        /// How many degrees will the spread 
        /// </summary>
        public Vector3 bulletSpread = new Vector3(0f, 0f, 0f);
        /// <summary>
        /// Knockback to apply to the player rigidbody, in this gameobject's local space
        /// </summary>
        public Vector3 recoil = new Vector3(0f, 0f, -20f);

        private Rigidbody playerRigidbody;

        void Start()
        {
            defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
            playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
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
                    for (int i = 0; i < numberOfBullets; i++)
                    {
                        GameObject bullet = ObjectPool.Instance.GetBullet();
                        bullet.GetComponent<Rigidbody>().position = transform.position + (CameraFocusTransform.rotation * bulletSpawnOffset);
                        bullet.GetComponent<Rigidbody>().velocity = CameraFocusTransform.rotation * Vector3.forward * bulletSpeed;
                    }
                    clip -= 1;

                    playerRigidbody.AddForce(transform.rotation * recoil, ForceMode.VelocityChange);
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
}
