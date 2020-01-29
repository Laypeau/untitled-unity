using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickupItem
{
    /// <summary>
    /// The base class for generic PickupItems
    /// </summary>
    public class PickupItem
    {
        
        public void PickUp()
        {

        }

        public void PutDown()
        {

        }

        public void Highlight()
        {

        }
    }

    public class UseItem : PickupItem
    {
        public float useTime = 0f;
        private float lastUseTime = 0f;

        public void Use()
        {
            if (Time.time >= lastUseTime + useTime)
            {
                PerformAction();
            }
        }

        public virtual void PerformAction()
        {
            Debug.Log("Base Class action");
        }
    }

    public class ProjectileWeapon : UseItem
    {
        public int clipSize;
        public int clip;
        public float reloadTime;
        public bool reloading = false;

        public ProjectileWeapon(float _useTime, int _clipSize, float _reloadTime)
        {
            useTime = _useTime;
            clipSize = _clipSize;
            clip = _clipSize;
            reloadTime = _reloadTime;
        }

        public override void PerformAction()
        {
            Debug.Log("Derived class action");
        }
    }

    public class MeleeWeapon : PickupItem
    {
        public float useTime;
        public float cooldownTime;

    }
}
