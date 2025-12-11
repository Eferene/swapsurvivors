using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunCharacter : BaseCharacterController
{
    [Header("Shotgun Stats")]
    [SerializeField] private float detectionRadius = 10.0f;
    [SerializeField] private float spreadAngle = 30.0f;
    [SerializeField] private float bulletSpeed = 20.0f;
    [SerializeField] private float bulletExplosionRadius = 1.0f;
    [SerializeField] private int bulletCount = 5;

    [Header("Components")]
    [SerializeField] private Transform WeaponHolder;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private Transform bulletsParent;
    private List<GameObject> bulletPool = new List<GameObject>(); // Mermi havuzu

    private void Start()
    {
        bulletsParent = new GameObject("BULLETS POOL").transform;
    }

    protected override float GetCooldown() => playerManager.CurrentCooldown;

    protected override void Update()
    {
        base.Update();

        // En yakın düşmanı bulur
        FindClosestEnemy();

        // Hedef varsa silahı hedefe döndürür
        if (currentTarget != null)
        {
            RotateWeaponToTarget();
        }
    }

    protected override void ApplyAttack()
    {
        if (currentTarget != null)
            base.ApplyAttack();
    }

    protected override void Attack()
    {
        switch (playerManager.CharacterLevel)
        {
            case 1:
                LevelOneAttack();
                break;
            case 2:
                LevelTwoAttack();
                break;
            case 3:
                LevelThreeAttack();
                break;
            default:
                LevelOneAttack();
                break;
        }
    }

    private GameObject GetBulletFromPool(Quaternion bulletRotation)
    {
        // Havuzu tara
        foreach (GameObject obj in bulletPool)
        {
            // Obje kullanılmıyorsa
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Hepsi kullanılıyorsa yeni obje oluştur
        GameObject newBullet = Instantiate(BulletPrefab, FirePoint.position, bulletRotation, bulletsParent);
        bulletPool.Add(newBullet);
        return newBullet;
    }

    private void FireBullet()
    {
        if (currentTarget == null) return;

        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);

            Quaternion aimRotation = FirePoint.rotation;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, aimRotation.eulerAngles.z + currentAngle);

            GameObject bullet = GetBulletFromPool(bulletRotation);

            bullet.transform.position = FirePoint.position;
            bullet.transform.rotation = bulletRotation;

            // Bullet bileşenini alıp gerekli ayarları yapar
            if (bullet.TryGetComponent(out ShotgunBullet bulletScript))
            {
                bulletScript.Setup(playerManager.GiveDamageCharacter(), bulletSpeed, playerManager.CurrentSpeed, bulletExplosionRadius,
                    (playerManager.GiveDamageCharacter() / 2), playerManager.CharacterLevel);
            }
        }
    }

    private void FindClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;
    }

    private void RotateWeaponToTarget()
    {
        if (currentTarget == null) return;

        // Düşmana giden yol vektörü
        Vector3 direction = currentTarget.position - WeaponHolder.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.MoveTowardsAngle(WeaponHolder.rotation.eulerAngles.z, targetAngle, 5f);

        WeaponHolder.rotation = Quaternion.Euler(0, 0, newAngle);

        // Silahın yukarı/aşağı yönünü ayarlar
        Vector3 localScale = Vector3.one;
        if (newAngle > 90 || newAngle < -90)
        {
            localScale.y = -1;
        }
        else
        {
            localScale.y = 1;
        }
        WeaponHolder.localScale = localScale;
    }

    private void LevelOneAttack()
    {
        FireBullet();
    }

    private void LevelTwoAttack()
    {
        StartCoroutine(DobuleShot(0.2f));
    }

    private void LevelThreeAttack()
    {
        StartCoroutine(DobuleShot(0.15f));
    }

    private IEnumerator DobuleShot(float t)
    {
        // İlk salvo
        FireBullet();

        yield return new WaitForSeconds(t);

        // İkinci salvo
        FireBullet();
    }

    private void OnDrawGizmosSelected()
    {
        if (playerManager == null)
            return;

        // Algılama menzili
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Mermi menzili
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(FirePoint.position, playerManager.CurrentRange);
    }
}
