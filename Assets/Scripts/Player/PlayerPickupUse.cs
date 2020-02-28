using System.Collections;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerPickupUse : MonoBehaviour
    {
        static LayerMask PickupMask;
        static Transform CameraFocusTransform;
        public static float pickupDistance = 5f;
        public static float pickupSphereSize = 0.2f;
        private GameObject PickedUpItem;

        void Start()
        {
            PickupMask = LayerMask.GetMask("PickupItem");
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (PickedUpItem != null)
                {
                    PickedUpItem.GetComponent<PickupItem.PickupItem>().Use();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                PickupPutdown();
            }
        }

        /// <summary>
        /// Picks up the item infront of the player or puts it down, depending on whether another pickupitem is being held
        /// </summary>
        public void PickupPutdown()
        {
            if (PickedUpItem == null)
            {
                Ray PickupRay = new Ray(CameraFocusTransform.position, CameraFocusTransform.rotation * Vector3.forward);

                if (Physics.SphereCast(PickupRay, pickupSphereSize, out RaycastHit RayHit, pickupDistance, PickupMask) && RayHit.collider.CompareTag("PickupItem"))
                {
                    PickedUpItem = RayHit.transform.gameObject;
                    PickedUpItem.GetComponent<PickupItem.PickupItem>().PickUp();
                }
            }
            else
            {
                PickedUpItem.GetComponent<PickupItem.PickupItem>().PutDown();
                PickedUpItem = null;
            }
        }
    }
}
