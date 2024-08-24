using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : BulletPool
{
    [Header("Bullet Settings")]
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 2f;

    [Header("Attack Settings")]
    [SerializeField] private int numberOfBulletsPerAttack = 1;
    [SerializeField] private float bulletInterval = 0.1f;
    [SerializeField] private int multipleShots = 1;
    [SerializeField] private float totalSpreadAngle = 15f;


    [Header("References for Bullet")]
    [SerializeField] private Transform bulletSpawnPoint;

    private void Update()
    {
    }
    
    public void Attack()
    {
        // Start firing bullets
        StartCoroutine(FireBullets());
    }

    private IEnumerator FireBullets()
    {
        for (int i = 0; i < numberOfBulletsPerAttack; i++)
        {
            for (int j = 0; j < multipleShots; j++)
            {
                float spreadAngle = totalSpreadAngle / (multipleShots - 1);
                float angle = -totalSpreadAngle / 2 + spreadAngle * j;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                GameObject bullet = GetBullet();
                bullet.transform.position = bulletSpawnPoint.position;
                bullet.transform.rotation = bulletSpawnPoint.rotation * rotation;
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.Initialize(bulletSpeed, bulletLifetime, 0f, null, 0f, this, BulletOrigin.Player);
                bulletScript.SetDamage(bulletDamage);
            }
            yield return new WaitForSeconds(bulletInterval);
        }
    }


}
