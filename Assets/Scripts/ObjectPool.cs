using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public GameObject BulletPrefab;
    public int NumberOfBullets = 5;
    public List<GameObject> bullets;

    void Awake()
    {
        Instance = this;

        bullets = new List<GameObject>(NumberOfBullets);

        for (int i = 0; i < NumberOfBullets; i++)
        {
            GameObject prefabInstance = Instantiate(BulletPrefab);
            prefabInstance.transform.SetParent(transform);
            prefabInstance.SetActive(false);

            bullets.Add(prefabInstance);
        }
        
    }

    public GameObject GetBullet()
    {
        foreach (GameObject bullet in bullets)
        {
            if (bullet.activeInHierarchy == false)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        GameObject prefabInstance = Instantiate(BulletPrefab);
        prefabInstance.transform.SetParent(transform);
        bullets.Add(prefabInstance);

        return prefabInstance;
    }
}
