using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : PickupItem.UseItem
{
    private Light spotLight;
    private bool torchOn = false;

    void Start()
    {
        spotLight = GetComponentInChildren<Light>();
    }

    public override void Use()
    {
        if (torchOn)
        {
            spotLight.enabled = false;
            torchOn = false;
        }
        else
        {
            spotLight.enabled = true;
            //Play click sound
            torchOn = true;
        }
    }
}
