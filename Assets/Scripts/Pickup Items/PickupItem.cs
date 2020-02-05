using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    /// <summary>
    /// The class attached to any gameobject that wants to be able to be picked up by the player
    /// </summary>
    public class PickupItem : MonoBehaviour
    {
        [HideInInspector] public bool hasUse = false;
        private bool PickedUp = false;
        private Vector3 playerOffset = new Vector3(0.69f, -0.25f, 0.3f);
        private Vector3 additionalDropForce = new Vector3(0f, 0f, 1f);
        private Rigidbody itemRigidbody;
        private MeshCollider itemMeshCollider;

        public UseItem useScript;
        private Transform playerTransform;
        private Transform cameraFocusTransform;
        
        void Awake()
        {
            itemRigidbody = GetComponent<Rigidbody>();
            itemMeshCollider = GetComponent<MeshCollider>();

            if (useScript != null)
            {
                hasUse = true;
                
            }
        }

        void Start()
        {
            playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            cameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
        }

        void Update()
        {
            //If player is within player.getcomponent<Pickup/Use>().PickupRange, highlight the item

            if (PickedUp)
            {
                transform.localPosition = playerOffset;
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        public void PickUp()
        {
            Destroy(itemRigidbody);
            itemMeshCollider.enabled = false;
            PickedUp = true;
            transform.SetParent(cameraFocusTransform);
        }

        public void PutDown()
        {
            itemRigidbody = gameObject.AddComponent<Rigidbody>();
            itemMeshCollider.enabled = true;
            PickedUp = false;
            itemRigidbody.velocity = GetComponentInParent<Rigidbody>().velocity + (cameraFocusTransform.rotation * additionalDropForce);
            transform.SetParent(null);
        }

        public void Use()
        {
            useScript.Use();
        }
    }

    /// <summary>
    /// The base class for any scripts that have a left click action when picked up
    /// </summary>
    public abstract class UseItem : MonoBehaviour
    {
        public static Transform CameraFocusTransform;
        public static Transform PlayerTransform;

        public void Awake()
        {
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
        }

        /// <summary>
        /// Override this method to add any functionality that will be triggered by MouseDown0 when being held
        /// </summary>
        public virtual void Use()
        {
            // Possibly just shake the item 
        }
    }
}
