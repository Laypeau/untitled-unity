using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    class Shotgun : UseItem
    {


        public override void Use()
        {
            Debug.Log("Shotgun fire");
        }
    }
}
