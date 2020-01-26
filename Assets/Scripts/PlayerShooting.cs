using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    Transform CameraFocusTransform;

    void Start()
    {
        CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = ObjectPool.Instance.GetBullet();
            //bullet.transform.rotation = CameraFocusTransform.rotation;
            bullet.transform.position = CameraFocusTransform.position + (CameraFocusTransform.rotation * new Vector3(0f, 0f, 1f));
            bullet.GetComponent<Rigidbody>().AddForce(25f * (CameraFocusTransform.rotation * Vector3.forward), ForceMode.VelocityChange);

        }
    }
}
