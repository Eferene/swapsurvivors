using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;

    private float currentHealth;
    private EnemyAttack attackType;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Material mainMat;
    private Transform wsCanvas;
    private PlayerManager playerManager;
    private SpriteRenderer spriteRenderer;

    // Cooldown
    [SerializeField] private float lastAttackTime;
    [SerializeField] private bool canAttack = true;
    public bool isAttacking = false;

    // Exploding
    public bool isExploding = false;

    private bool isCritical = false;

    [Header("Resources")]
    [SerializeField] Material flashMat;
    [SerializeField] TextMeshProUGUI damageTMP;
    [SerializeField] GameObject effectPrefab;

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        attackType = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMat = spriteRenderer.material;
        spriteRenderer.material = mainMat;
        wsCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas").transform;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
        playerManager = playerObj.GetComponent<PlayerManager>();
    }

    void OnEnable()
    {
        currentHealth = enemyData.baseHealth;
        canAttack = true;
        isAttacking = false;
        spriteRenderer.material = mainMat;
        playerManager.OnDamageHitOccurred += IsCritical;
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
            bool attackSuccessful = attackType.Attack(transform, playerTransform, enemyData.attackDamage, enemyData.attackDamagePercentage, enemyData.attackRange);
            if (attackSuccessful)
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
            case SuicideAttack _:
            case MeleeAttack _:
                MoveTowardsPlayer();
                break;
            case RaycastAttack _:
            case PredictiveProjectileAttack _:
            case HomingMissileAttack _:
            case ThrowAttack _:
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

        if (rb != null && playerTransform != null && enemyData != null && !isExploding)
        {
            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)playerTransform.position;
            Vector2 direction = (targetPos - currentPos).normalized;
            if (direction.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            rb.linearVelocity = direction * enemyData.speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void KeepDistanceMovement()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (rb != null && playerTransform != null && enemyData != null && Vector2.Distance(rb.position, playerTransform.position) > enemyData.attackRange)
        {
            if (isAttacking) return;

            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)playerTransform.position;
            Vector2 direction = (targetPos - currentPos).normalized;

            rb.linearVelocity = direction * enemyData.speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (!IsDead) StartCoroutine(Flash());
        AudioManager.Instance.PlayEnemyHurtSFX();

        TextMeshProUGUI newText = Instantiate(damageTMP, wsCanvas);
        newText.text = damage.ToString();
        if (isCritical) newText.color = Color.red;
        else newText.color = Color.white;
        newText.transform.position = transform.position + new Vector3(0, 1.5f, 0f);

        if (currentHealth <= 0)
            Die();
    }

    private void IsCritical(bool critical) => isCritical = critical;

    private void Die()
    {
        float min = enemyData.scoreGain * (1f - enemyData.scoreGainPercentage / 100f);
        float max = enemyData.scoreGain * (1f + enemyData.scoreGainPercentage / 100f);
        float fScoreGain = Random.Range(min, max);
        int scoreGain = Mathf.RoundToInt(fScoreGain);
        playerManager.AddScore(scoreGain);
        DieEffect();
        GetComponent<SpriteRenderer>().material = mainMat;
        EnemyPool.Instance.ReturnEnemyToPool(this.gameObject);
    }

    public void DieEffect()
    {
        GameObject newEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        var mainSettings = newEffect.GetComponent<ParticleSystem>().main;
        mainSettings.startColor = new ParticleSystem.MinMaxGradient(enemyData.colors[0], enemyData.colors[1]);

        Destroy(newEffect, 1f);
    }

    private IEnumerator Flash()
    {
        GetComponent<SpriteRenderer>().material = flashMat;
        yield return new WaitForSeconds(0.075f);
        GetComponent<SpriteRenderer>().material = mainMat;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + enemyData.attackOffset, enemyData != null ? enemyData.attackRange : 1f);
    }
}