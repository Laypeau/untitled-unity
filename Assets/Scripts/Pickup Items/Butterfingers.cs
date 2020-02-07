using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    public class Butterfingers : UseItem
    {
        public override void Use()
        {
            GameObject.Find("Player").GetComponent<PlayerControl.PlayerPickupUse>().PickupPutdown();
        }
    }
}