using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PickupItem;

namespace PlayerControl
{
    public class PlayerPickupUse : MonoBehaviour
    {
        static LayerMask PickupMask;
        void Start()
        {
            PickupMask = LayerMask.GetMask("PickupItem");
        }

        ProjectileWeapon Pistol = new ProjectileWeapon(0.4f, 10, 4f);

        void Update()
        {
            //Outline the pickuppable item

            //if lmb pressed
                //if item equipped
                    //Use()
                //Else if raycast is hitting an item to pickup
                    //PickUp()

            //if key E pressed and item equipped
                //PutDown()

            if (Input.GetMouseButtonDown(0))
            {
                Pistol.Use();
            }
        }
    }
}
