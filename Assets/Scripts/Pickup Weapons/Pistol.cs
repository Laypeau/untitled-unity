using UnityEngine;
using System;

namespace PickupItem
{
    class Pistol : MonoBehaviour
    {
        Transform CameraFocusTransform;
        Transform PlayerTransform;

        public int clipSize = 10;
        public int clip = 10;
        public float reloadTime = 4f;
        public bool reloading = false;

        float bulletSpeed = 20f;

        public float useTime = 0.2f;
        public float lastUsed = 0f;

        void Awake()
        {
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            if (clip > 0 )
            {
                if (Time.time >= lastUsed + useTime)
                {
                    GameObject bullet = ObjectPool.Instance.GetBullet();
                    bullet.GetComponent<Rigidbody>().position = PlayerTransform.position;
                    bullet.GetComponent<Rigidbody>().AddForce(CameraFocusTransform.rotation * Vector3.forward * bulletSpeed, ForceMode.VelocityChange);
                    clip -= 1;
                }
            }
            else
            {
                if (!reloading)
                {
                    Debug.Log("reloading");
                    reloading = true;
                    Invoke("Reload", reloadTime);
                }
            }
        }

        private void Reload()
        {
            clip = clipSize;
            reloading = false;
            Debug.Log("Reloaded!");
        }
    }

    interface IProjectileWeapon
    {

    }
}