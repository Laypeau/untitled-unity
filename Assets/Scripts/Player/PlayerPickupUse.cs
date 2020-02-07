using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerPickupUse : MonoBehaviour
    {
        static LayerMask PickupMask;
        static Transform CameraFocusTransform;
        public static float PickupDistance = 5f;
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
        /// Either picks up the item infront of the player or puts it down
        /// </summary>
        public void PickupPutdown()
        {
            if (PickedUpItem == null)
            {
                Ray PickupRay = new Ray(CameraFocusTransform.position, CameraFocusTransform.rotation * Vector3.forward);

                if (Physics.Raycast(PickupRay, out RaycastHit RayHit, PickupDistance, PickupMask) && RayHit.collider.CompareTag("PickupItem"))
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
