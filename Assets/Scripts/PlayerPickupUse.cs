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
            //Outline the pickuppable item if it is within pickup range of the player

            Ray PickupRay = new Ray(CameraFocusTransform.position, CameraFocusTransform.rotation * Vector3.forward);

            if (Input.GetMouseButtonDown(0))
            {
                if (PickedUpItem != null)
                {
                    PickedUpItem.GetComponent<PickupItem.Pistol>().Shoot();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PickedUpItem == null)
                {
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
}
