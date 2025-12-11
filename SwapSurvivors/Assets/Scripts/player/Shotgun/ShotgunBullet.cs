
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    private float bulletDamage;
    private float bulletRange;
    private float bulletExplosionRange;
    private float bulletExplosionDamage;
    private float characterLevel;
    private Vector3 startPosition;

    Rigidbody2D rb;

    // Bu metod, mermi özelliklerini ayarlamak için çağrılır
    public void Setup(float bulletDamage, float bulletSpeed, float bulletRange, float bulletExplosionRange, float bulletExplosionDamage, int characterLevel)
    {
        this.bulletDamage = bulletDamage;
        this.bulletRange = bulletRange;
        this.bulletExplosionRange = bulletExplosionRange;
        this.bulletExplosionDamage = bulletExplosionDamage;
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
                enemyController.TakeDamage(bulletDamage);
                //Debug.Log($"{collision.name} gelen {bulletDamage} hasarı yedi.");

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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bulletExplosionRange);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                if (enemy.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.TakeDamage(bulletExplosionDamage);
                    //Debug.Log($"{enemy.name} gelen {bulletExplosionDamage} hasarı yedi.");
                }
            }
        }
    }
}