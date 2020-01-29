using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public GameObject BulletPrefab;
    public int NumberOfBullets = 5;
    public List<GameObject> BulletsInExistence;

    void Awake()
    {
        Instance = this;

        BulletsInExistence = new List<GameObject>(NumberOfBullets);

        for (int i = 0; i < NumberOfBullets; i++)
        {
            GameObject prefabInstance = Instantiate(BulletPrefab);
            prefabInstance.transform.SetParent(transform);
            prefabInstance.SetActive(false);

            BulletsInExistence.Add(prefabInstance);
        }
        
    }

    public GameObject GetBullet()
    {
        foreach (GameObject bullet in BulletsInExistence)
        {
            if (bullet.activeInHierarchy == false)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        GameObject prefabInstance = Instantiate(BulletPrefab);
        prefabInstance.transform.SetParent(transform);
        BulletsInExistence.Add(prefabInstance);

        return prefabInstance;
    }
}
