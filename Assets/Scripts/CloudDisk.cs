using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDisk : MonoBehaviour
{
    [SerializeField] private GameObject playerGameobject;

    void Update()
    {
        transform.position = new Vector3(playerGameobject.transform.position.x, transform.position.y, playerGameobject.transform.position.z);
    }
}
