using UnityEngine;
using System;

namespace PickupItem
{
    class Pistol : MonoBehaviour, IProjectileWeapon
    {
        Transform CameraFocusTransform;
        Transform PlayerTransform;
        GameObject ParentedPickupObject;

        //temp
        public Material defaultMaterial;
        public Material reloadMaterial;

        public int clipSize = 10;
        public int clip = 10;
        public float reloadTime = 3f;
        public bool reloading = false;
        public float useTime = 0.2f;
        [HideInInspector]
        private float lastUsed = 0f;

        float bulletSpeed = 30f;

        void Awake()
        {
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
        }

        public void Shoot()
        {
            if (clip > 0 )
            {
                if (Time.time >= lastUsed + useTime)
                {
                    GameObject bullet = ObjectPool.Instance.GetBullet();
                    bullet.GetComponent<Rigidbody>().position = CameraFocusTransform.rotation * (transform.position + new Vector3(0f, 0f, 0.4f));
                    bullet.GetComponent<Rigidbody>().velocity = CameraFocusTransform.rotation * Vector3.forward * bulletSpeed;
                    clip -= 1;
                }
            }
            else
            {
                if (!reloading)
                {
                    Debug.Log("reloading");
                    //ParentedPickupObject.GetComponent<MeshRenderer>().material = 
                    reloading = true;
                    Invoke("Reload", reloadTime);
                }
            }
        }

        public void Reload()
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