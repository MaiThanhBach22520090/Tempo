using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType { Straight, Spin, SpawnAroundTarget }
    enum SpawnPosition { Origin, Random }
    enum RotationDirection { None, Left, Right }
    enum PatternShape { Round, Diamond }

    [Header("Bullet Attributes")]
    public float bulletLife = 1f;
    public float speed = 1f;
    [SerializeField] private float delayBeforeMoving = 1f;
    [SerializeField] private float rotateTime = 1f;

    [Header("Spawner Attributes")]
    [SerializeField] private bool allowFire = true;
    [SerializeField] private Transform target;
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private SpawnPosition spawnPosition;
    [SerializeField] private RotationDirection rotationDirection;
    [SerializeField] private PatternShape patternShape;
    [SerializeField] private float firingRate = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private int multipleBullets = 1;
    [SerializeField] private bool randomizeBulletRotation = false;

    [Header("Bullet Pattern Attributes")]
    [SerializeField] private float diamondShapeWidth;
    [SerializeField] private float diamondShapeHeight;
    [SerializeField] private bool rotateWhenFire = false;
    [SerializeField] private float rotationDegree = 10f;


    [Header("Play Field")]
    [SerializeField] private Transform playFieldOrigin;
    [SerializeField] private float spawnAreaWidth = 10f;
    [SerializeField] private float spawnAreaHeight = 10f;

    private BulletPool bulletPool;
    private float timer = 0f;

    private void Start()
    {
        bulletPool = GetComponent<BulletPool>();
        if (bulletPool == null)
        {
            Debug.LogError("BulletPool component not found on the GameObject!");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (allowFire)
        {
            if (timer >= firingRate)
            {
                if (spawnerType == SpawnerType.Straight || spawnerType == SpawnerType.Spin)
                {
                    Fire();
                }
                else if (spawnerType == SpawnerType.SpawnAroundTarget)
                {
                    FireAroundTarget();
                }
                timer = 0f;
            }
        }

        if (rotationDirection != RotationDirection.None && !rotateWhenFire)
        {
            float directionMultiplier = rotationDirection == RotationDirection.Left ? -1f : 1f;
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + rotationSpeed * directionMultiplier * Time.deltaTime);
        }
    }

    public void Fire()
    {
        if (multipleBullets > 0)
        {
            switch (patternShape)
            {
                case PatternShape.Round:
                    FireInRoundShape();
                    break;
                case PatternShape.Diamond:
                    FireInDiamondShape();
                    break;
            }

            if (rotateWhenFire)
            {
                float directionMultiplier = rotationDirection == RotationDirection.Left ? -1f : 1f;
                transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + rotationDegree * directionMultiplier);
            }
        }
    }

    private void FireInRoundShape()
    {
        float angleStep = 360f / multipleBullets;
        float angle = 0f;

        Vector2 spawnPoint = GetSpawnPoint();

        for (int i = 0; i < multipleBullets; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + angle));
            angle += angleStep;

            GameObject spawnedBullet = bulletPool.GetBullet();
            spawnedBullet.transform.position = spawnPoint;
            spawnedBullet.transform.rotation = bulletRotation;

            Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
            bulletScript.Initialize(speed, bulletLife, 0, null, 0, bulletPool, BulletOrigin.Enemy);
        }
    }

    private void FireInDiamondShape()
    {
            // Get spawn point
            Vector2 spawnPoint = GetSpawnPoint();

            // Determine the number of bullets to place at the diamond shape's vertices
            float angleStep = 360f / multipleBullets;

            // set Position of the each Bullet
            for (int i = 0; i < multipleBullets; i++)
            {
                float angle = i * angleStep;
                Vector2 offset = GetDiamondOffset(angle);

                Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + angle));

                GameObject spawnedBullet = bulletPool.GetBullet();
                spawnedBullet.transform.position = spawnPoint + offset;
                spawnedBullet.transform.rotation = bulletRotation;

                Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
                bulletScript.Initialize(speed, bulletLife, 0, null, 0, bulletPool, BulletOrigin.Enemy);
            }

    }

    private Vector2 GetDiamondOffset(float angle)
    {
        // Calculate the offset based on the angle and the diamond shape's width and height
        float radians = angle * Mathf.Deg2Rad;
        float x = diamondShapeWidth * Mathf.Cos(radians);
        float y = diamondShapeHeight * Mathf.Sin(radians);

        return new Vector2(x, y);
    }


    private Vector2 GetSpawnPoint()
    {
        if (spawnPosition == SpawnPosition.Origin)
        {
            return transform.position;
        }
        else if (spawnPosition == SpawnPosition.Random)
        {
            return new Vector2(
                Random.Range(playFieldOrigin.position.x - spawnAreaWidth / 2, playFieldOrigin.position.x + spawnAreaWidth / 2),
                Random.Range(playFieldOrigin.position.y - spawnAreaHeight / 2, playFieldOrigin.position.y + spawnAreaHeight / 2)
            );
        }
        return Vector2.zero;
    }

    private void FireAroundTarget()
    {
        if (multipleBullets > 0 && target != null)
        {
            float angleStep = 360f / multipleBullets;
            float angle = 0f;
            if (randomizeBulletRotation)
            {
                angle = Random.Range(0f, 360f);
            }

            for (int i = 0; i < multipleBullets; i++)
            {
                angle += angleStep;

                float radians = angle * Mathf.Deg2Rad;
                Vector2 spawnPosition = new Vector2(
                    target.position.x + Mathf.Cos(radians) * spawnRadius,
                    target.position.y + Mathf.Sin(radians) * spawnRadius
                );

                Vector2 direction = (target.position - (Vector3)spawnPosition).normalized;
                float angleToTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                GameObject spawnedBullet = bulletPool.GetBullet();
                spawnedBullet.transform.position = spawnPosition;
                spawnedBullet.transform.rotation = Quaternion.Euler(0, 0, angleToTarget);

                Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
                bulletScript.Initialize(speed, bulletLife, delayBeforeMoving, target, rotateTime, bulletPool, BulletOrigin.Enemy);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playFieldOrigin != null && spawnPosition == SpawnPosition.Random)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(playFieldOrigin.position, new Vector3(spawnAreaWidth, spawnAreaHeight, 0));
        }

        // Draw the diamond shape using lines from transform position to the bullet's position
        if (patternShape == PatternShape.Diamond)
        {
            float angleStep = 360f / multipleBullets;
            float angle = 0f;

            Vector2 spawnPoint = GetSpawnPoint();

            for (int i = 0; i < multipleBullets; i++)
            {
                Vector2 offset = GetDiamondOffset(angle);
                angle += angleStep;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(spawnPoint, spawnPoint + offset);
            }
        }

    }
}
