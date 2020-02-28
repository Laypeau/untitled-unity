using System.Collections;
using UnityEngine;

// Integrate more with PlayerPickupUse!!!

namespace PickupItem
{
    /// <summary>
    /// The class attached to any gameobject that wants to be able to be picked up by the player
    /// </summary>
    public class PickupItem : MonoBehaviour
    {
        [HideInInspector] public bool hasUse = false;
        [HideInInspector] public bool PickedUp = false;
        public bool highlightItem = true;
        /// <summary>
        /// Offset of the PickupItem from the camerafocus transform when picked up
        /// </summary>
        public Vector3 equippedOffset = new Vector3(0.7f, -0.3f, 0.5f);
        /// <summary>
        /// Rotation of the PickupItem from the camerafocus transform when picked up
        /// </summary>
        public Vector3 equippedRotation = new Vector3(0f, 0f, 0f);
        public Vector3 dropForce = new Vector3(0f, 0f, 2f);

        [HideInInspector] public Rigidbody itemRigidbody;
        /// <summary>
        /// By default is some kind of mesh collider, but can be overridden
        /// </summary>
        [HideInInspector] public Collider itemCollider;

        public UseItem useScript;
        private Transform playerTransform;
        private Transform cameraFocusTransform;
        
        void Awake()
        {
            itemRigidbody = GetComponent<Rigidbody>();
            itemCollider = GetComponent<Collider>();
        }

        void Start()
        {
            playerTransform = GameObject.Find("Player").GetComponent<Transform>();
            cameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();

            if (useScript == null)
            {
                useScript = gameObject.AddComponent<UseItem>();
            }
            else
            {
                hasUse = true;
            }
        }

        void Update()
        {
            if (highlightItem)
            {
                //If player is within player.getcomponent<Pickup/Use>().PickupRange, highlight the item
            }
        }

        public void PickUp()
        {
            Destroy(itemRigidbody);
            itemCollider.enabled = false;
            PickedUp = true;

            transform.SetParent(cameraFocusTransform);
            transform.localPosition = equippedOffset;
            transform.localRotation = Quaternion.Euler(equippedRotation);
        }

        public void PutDown()
        {
            itemRigidbody = gameObject.AddComponent<Rigidbody>();
            itemCollider.enabled = true;
            PickedUp = false;
            itemRigidbody.AddForce(playerTransform.gameObject.GetComponent<Rigidbody>().velocity + (transform.rotation * dropForce), ForceMode.VelocityChange);
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
    public class UseItem : MonoBehaviour
    {
        public static PickupItem PickupItem;
        public static Transform CameraFocusTransform;
        public static Transform PlayerTransform;

        void Awake() 
        {
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
            PickupItem = GetComponent<PickupItem>();
        }

        /// <summary>
        /// Default functionality is to add 3 to camera shake trauma
        /// </summary>
        public virtual void Use()
        {
            CameraFocusTransform.GetComponent<CameraController>().AddShakeTrauma(3f);
        }
    }
}
