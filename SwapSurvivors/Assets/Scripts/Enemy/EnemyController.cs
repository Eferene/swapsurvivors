using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float currentHealth;
    public EnemyData enemyData;
    private BaseAttackType attackType;
    Rigidbody2D rb;
    Transform playerTransform;

    // Cooldown
    [SerializeField] float lastAttackTime;
    [SerializeField] bool canAttack = true;
    public bool isAttacking = false;

    void Start()
    {
        currentHealth = enemyData.baseHealth;
        attackType = GetComponent<BaseAttackType>();
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        // Cooldown ile vurup vuramayacağını kontrol etme.
        if (Time.time - lastAttackTime >= enemyData.attackCooldown)
        {
            canAttack = true;
        }

        // Basit saldırı
        if (attackType != null && canAttack && !isAttacking)
        {
            bool attackSuccessful = attackType.Attack(transform, GameObject.FindGameObjectWithTag("Player").transform, enemyData.attackDamage, enemyData.attackDamagePercentage, enemyData.attackRange);
            if(attackSuccessful)
            {
                lastAttackTime = Time.time;
                canAttack = false;
            }
        }
    }

    void FixedUpdate()
    {
        switch (attackType)
        {
            case MeleeAttack _:
                MoveTowardsPlayer();
                break;
            case RaycastAttack _:
                KeepDistanceMovement();
                break;
            case ProjectileAttack _:
                KeepDistanceMovement();
                break;
            default:
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (rb != null && playerTransform != null && enemyData != null)
        {
            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)playerTransform.position;
            Vector2 direction = (targetPos - currentPos).normalized;

            rb.MovePosition(currentPos + direction * enemyData.speed * Time.fixedDeltaTime);
        }
    }

    void KeepDistanceMovement()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (rb != null && playerTransform != null && enemyData != null && Vector2.Distance(rb.position, playerTransform.position) > enemyData.attackRange - 0.3f)
        {
            if(isAttacking) return;

            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)playerTransform.position;
            Vector2 direction = (targetPos - currentPos).normalized;

            rb.MovePosition(currentPos + direction * enemyData.speed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log($"{enemyData.enemyName} defeated!");
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData != null ? enemyData.attackRange : 1f);
    }
}