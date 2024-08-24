using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Pool Settings")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Color bulletColor = Color.white;
    [SerializeField] protected int poolSize = 20;  // Number of bullets to instantiate initially
    [SerializeField] protected Transform poolParent;

    protected Queue<GameObject> pool;

    protected void Awake()
    {
        Instance = this;
        InitializePool();
    }

    protected virtual void InitializePool()
    {
        pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, poolParent);
            bullet.GetComponentInChildren<SpriteRenderer>().color = bulletColor;
            bullet.SetActive(false);  // Deactivate the bullet
            pool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (pool.Count > 0)
        {
            GameObject bullet = pool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            // If no bullets are available in the pool, instantiate a new one (optional)
            GameObject newBullet = Instantiate(bulletPrefab, poolParent);
            newBullet.GetComponentInChildren<SpriteRenderer>().color = bulletColor;
            newBullet.SetActive(true);
            return newBullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);  // Deactivate the bullet
        pool.Enqueue(bullet);
    }
}
