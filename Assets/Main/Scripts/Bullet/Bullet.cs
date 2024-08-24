using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private float bulletLife;
    private float speed;
    private int damage;
    private float delayBeforeMoving;
    private Vector2 targetPosition;
    private float rotateTime;
    private Vector2 direction;
    private bool moveToTarget = false;
    private BulletPool pool;
    private BulletOrigin origin;


    public void Initialize(float speed, float bulletLife, float delayBeforeMoving, Transform targetTransform, float rotateTime, BulletPool pool, BulletOrigin origin)
    {
        this.speed = speed;
        this.bulletLife = bulletLife;
        this.delayBeforeMoving = delayBeforeMoving;
        this.rotateTime = rotateTime;
        this.pool = pool;
        this.origin = origin;

        if (targetTransform != null)
        {
            this.targetPosition = new Vector2(targetTransform.position.x, targetTransform.position.y);
            direction = (this.targetPosition - (Vector2)transform.position).normalized;
        }
        else
        {
            direction = transform.right;
        }

        if (rotateTime > 0f)
        {
            StartCoroutine(RotateAroundTarget());
        }
        else if (delayBeforeMoving > 0f)
        {
            StartCoroutine(DelayBeforeMoving());
        }
        else
        {
            moveToTarget = true;
        }
    }

    private void Update()
    {
        bulletLife -= Time.deltaTime;
        if (bulletLife <= 0f)
        {
            // add to dodged bullets
            if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.IncreaseDodgedBullets();

            // Return the bullet to the pool
            ReturnToPool();
        }

        if (moveToTarget)
        {
            Move();
        }
    }

    private IEnumerator RotateAroundTarget()
    {
        float elapsedTime = 0f;
        moveToTarget = false;

        while (elapsedTime < rotateTime)
        {
            elapsedTime += Time.deltaTime;

            // Rotate around the target position
            transform.RotateAround(targetPosition, Vector3.forward, 360f * Time.deltaTime / rotateTime);

            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeMoving);
        moveToTarget = true;
    }

    private IEnumerator DelayBeforeMoving()
    {
        moveToTarget = false;
        yield return new WaitForSeconds(delayBeforeMoving);
        moveToTarget = true;
    }

    private void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        // Deactivate the bullet and return it to the pool
        pool.ReturnBullet(gameObject);
    }

    public void SetDamage(int damage)
    {
        // Set the damage value
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Player>() && origin == BulletOrigin.Enemy)
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamage(1);

            ScoreSystem.Instance.ResetCombo();
            ScoreSystem.Instance.IncreaseHitBullets();
        }
        
        // Deal damage to the enemy
        if (collision.gameObject.GetComponent<Enemy>() && origin == BulletOrigin.Player)
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
            ScoreSystem.Instance.IncreaseDamageDealed(damage);
        }


        // Ignore collision with other bullets
        if (collision.gameObject.GetComponent<Bullet>())
        {
            return;
        }

        // If origin is player, ignore collision with player
        if (collision.gameObject.GetComponent<Player>() && origin == BulletOrigin.Player)
        {
            return;
        }

        // If origin is enemy, ignore collision with enemy
        if (collision.gameObject.GetComponent<Enemy>() && origin == BulletOrigin.Enemy)
        {
            return;
        }

        // add to dodged bullets if bullet's origin is enemy
        if (origin == BulletOrigin.Enemy)
        {
            ScoreSystem.Instance.IncreaseDodgedBullets();
        }

        // Return the bullet to the pool
        ReturnToPool();

    }

    // add dodged bullets to the score when the bullet is out of the screen
    private void OnBecameInvisible()
    {
        ScoreSystem.Instance.IncreaseDodgedBullets();
        ReturnToPool();
    }
}

public enum BulletOrigin
{
    Player,
    Enemy
}