using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    private float boomerangDamage;
    private float boomerangRange;
    private float boomerangSpeed;

    private Vector3 startPosition;
    private GameObject boomerangCharacter;
    private Rigidbody2D rb;

    private bool isReturning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Bu metod, bumerang özelliklerini ayarlamak için çağrılır
    public void Setup(float boomerangDamage, float boomerangRange, float boomerangSpeed, GameObject boomerangCharacter)
    {
        this.boomerangDamage = boomerangDamage;
        this.boomerangRange = boomerangRange;
        this.boomerangSpeed = boomerangSpeed;
        this.boomerangCharacter = boomerangCharacter;
        startPosition = transform.position;

        isReturning = false;

        // Hız ayarı
        rb.linearVelocity = transform.right * boomerangSpeed;
    }

    private void ReturnToPool(GameObject obj)
    {
        rb.linearVelocity = Vector2.zero;
        isReturning = false;
        obj.SetActive(false);
    }

    private void Update()
    {
        transform.Rotate(0, 0, -720 * Time.deltaTime); // Bumerang dönme

        // Menzili aştıysa geri dön flagini ayarla
        if (!isReturning && Vector3.Distance(startPosition, transform.position) >= boomerangRange)
            isReturning = true;

        // Geri dönüyorsa oyuncuya doğru hareket et
        if (isReturning)
        {
            Vector3 directionToPlayer = (boomerangCharacter.transform.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * boomerangSpeed;

            // Sadece geri dönüyorsa mesafeyi kontrol et
            if (Vector2.Distance(boomerangCharacter.transform.position, transform.position) < 0.5f)
                ReturnToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Düşmanla çarpışma kontrolü
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyController enemyController))
            {
                if (enemyController.IsDead) return;
                enemyController.TakeDamage(boomerangDamage);
            }
        }
    }
}
