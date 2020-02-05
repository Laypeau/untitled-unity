using UnityEngine;
using System;
using PickupItem;

namespace PickupItem
{
    class Pistol : UseItem
    {
        GameObject PickupGameObject;

        //temp
        private Material defaultMaterial;
        public Material reloadMaterial;

        public int clipSize = 10;
        public int clip = 10;
        public float reloadTime = 3f;
        public bool reloading = false;
        public float useTime = 0.2f;
        [HideInInspector]
        private float lastUsed = 0f;

        float bulletSpeed = 30f;

        void Start()
        {
            defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
        }

        public override void Use()
        {
            if (clip > 0 && !reloading)
            {
                if (Time.time >= lastUsed + useTime)
                {
                    GameObject bullet = ObjectPool.Instance.GetBullet();
                    bullet.GetComponent<Rigidbody>().position = CameraFocusTransform.rotation * (transform.position + new Vector3(0f, 0f, 0.4f));
                    bullet.GetComponent<Rigidbody>().velocity = CameraFocusTransform.rotation * Vector3.forward * bulletSpeed;
                    clip -= 1;
                }
            }
            
            if (clip <= 0 || Input.GetKeyDown(KeyCode.R))
            {
                if (!reloading)
                {
                    gameObject.GetComponent<MeshRenderer>().material = reloadMaterial;
                    reloading = true;
                    Invoke("Reload", reloadTime);
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