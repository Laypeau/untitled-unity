using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    public class PickupItem : MonoBehaviour
    {
        private bool PickedUp = false;

        private Vector3 playerOffset = new Vector3(0.5f, 0.1f, 0.5f);
        private Vector3 additionalDropForce = new Vector3(0f, 0f, 1f);
        private Rigidbody itemRigidbody;
        private MeshCollider itemMeshCollider;

        private Transform playerTransform;
        private Transform cameraFocusTransform;

        void Awake()
        {
            itemRigidbody = GetComponent<Rigidbody>();
            itemMeshCollider = GetComponent<MeshCollider>();
        }

        void Start()
        {
            playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            cameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
        }

        public void PickUp()
        {
            Destroy(itemRigidbody);
            itemMeshCollider.enabled = false;
            PickedUp = true;
            transform.SetParent(playerTransform);
        }

        public void PutDown()
        {
            itemRigidbody = gameObject.AddComponent<Rigidbody>();
            itemMeshCollider.enabled = true;
            PickedUp = false;
            itemRigidbody.velocity = GetComponentInParent<Rigidbody>().velocity + (cameraFocusTransform.rotation * additionalDropForce);
            transform.SetParent(null);
        }

        void Update()
        {
            //If player is within player.getcomponent<Pickup/Use>().PickupRange, highlight the item

            if (PickedUp)
            {
                transform.localPosition = cameraFocusTransform.rotation * playerOffset;
                transform.localRotation = cameraFocusTransform.rotation * Quaternion.Euler(-90f, 0f, 0f);
            }
        }
    }
}
