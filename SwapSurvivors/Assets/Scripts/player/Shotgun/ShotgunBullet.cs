
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    private float bulletDamage;
    private float bulletRange;
    private float bulletExplosionRadius;
    private float bulletExplosionDamage;
    private float characterLevel;
    private Vector3 startPosition;

    private PlayerManager playerManager;
    private Rigidbody2D rb;

    // Bu metod, mermi özelliklerini ayarlamak için çağrılır
    public void Setup(PlayerManager playerManager, float bulletSpeed, float bulletRange, float bulletExplosionRadius, int characterLevel)
    {
        this.playerManager = playerManager;
        this.bulletRange = bulletRange;
        this.bulletExplosionRadius = bulletExplosionRadius;
        this.characterLevel = characterLevel;

        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        // Hız ayarı
        if (rb != null)
            rb.linearVelocity = transform.right * bulletSpeed;
    }

    private void ReturnToPool(GameObject obj)
    {
        rb.linearVelocity = Vector2.zero;
        obj.SetActive(false);
    }

    private void Update()
    {
        // Mermi menzilini aştıysa yok et
        if (Vector3.Distance(startPosition, transform.position) >= bulletRange)
            ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Düşmanla çarpışma kontrolü
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyController enemyController))
            {
                if (enemyController.IsDead) return;
                bulletDamage = playerManager.CalculateDamage();
                enemyController.TakeDamage(bulletDamage);
                playerManager.ApplyOnHitEffects(bulletDamage);

                if (characterLevel == 3)
                    BulletExplosion();

                ReturnToPool(gameObject);
            }
        }

        // Mermi engelle çarpıştığında yok et
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (characterLevel == 3)
                BulletExplosion();
            ReturnToPool(gameObject);
        }
    }

    private void BulletExplosion()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bulletExplosionRadius);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                if (enemy.TryGetComponent(out EnemyController enemyController))
                {
                    bulletExplosionDamage = playerManager.CalculateDamage() * 0.5f;
                    enemyController.TakeDamage(bulletExplosionDamage);
                    playerManager.ApplyOnHitEffects(bulletExplosionDamage);
                }
            }
        }
    }
}