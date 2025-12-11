using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    #region Variables

    [Header("Movement Settings")]
    [SerializeField] private float projectileDuration = 3.0f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float moveTime = 0.1f; // Ne kadar süre hareket edip duracak

    private float damage;
    private Rigidbody2D rb;

    #endregion

    #region Unity Methods
    private void Awake() => rb = GetComponent<Rigidbody2D>();
    private void Update() => transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<EnemyController>(out EnemyController enemyController))
            {
                if (!enemyController.IsDead)
                    enemyController.TakeDamage(damage);
            }
        }
    }
    #endregion

    #region Setup
    public void Setup(float damage, float speed, Vector3 direction)
    {
        this.damage = damage;

        // Yön ayarı
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Hız ayarı
        rb.linearVelocity = direction * speed;

        // Hareketi durdur ve yok etme zamanlaması
        StartCoroutine(StopMovingRoutine());
        Destroy(gameObject, projectileDuration);
    }

    private IEnumerator StopMovingRoutine()
    {
        yield return new WaitForSeconds(moveTime);
        rb.linearVelocity = Vector2.zero;
    }
    #endregion
}
