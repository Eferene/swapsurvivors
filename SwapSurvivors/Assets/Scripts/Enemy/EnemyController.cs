using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float currentHealth;
    public EnemyData enemyData;
    private EnemyAttack attackType;
    Rigidbody2D rb;
    Transform playerTransform;
    Material mainMat;
    Transform wsCanvas;

    // Cooldown
    [SerializeField] float lastAttackTime;
    [SerializeField] bool canAttack = true;
    public bool isAttacking = false;

    // Resources
    Material flashMat;
    TextMeshProUGUI damageTMP;
    GameObject effectPrefab;

    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = enemyData.baseHealth;
        attackType = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
        flashMat = Resources.Load<Material>("HitFlashMaterial");
        damageTMP = Resources.Load<TextMeshProUGUI>("Damage Text");
        mainMat = GetComponent<SpriteRenderer>().material;
        wsCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas").transform;
        effectPrefab = Resources.Load<GameObject>("DeathEffect");
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

        if (rb != null && playerTransform != null && enemyData != null)
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
        StartCoroutine(Flash());
        AudioManager.Instance.PlayEnemyHurtSFX();

        TextMeshProUGUI newText = Instantiate(damageTMP, wsCanvas);
        newText.text = damage.ToString();
        newText.color = Color.white;
        newText.transform.position = transform.position + new Vector3(0, 1.5f, 0f);

        if (currentHealth <= 0)
        {
            float min = enemyData.scoreGain * (1f - enemyData.scoreGainPercentage / 100f);
            float max = enemyData.scoreGain * (1f + enemyData.scoreGainPercentage / 100f);
            float fScoreGain = Random.Range(min, max);
            int scoreGain = Mathf.RoundToInt(fScoreGain);
            PlayerStats.Instance.AddScore(scoreGain);
            UIController.Instance.UpdateScoreText();
            GameObject newEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            var mainSettings = newEffect.GetComponent<ParticleSystem>().main;
            mainSettings.startColor = new ParticleSystem.MinMaxGradient(enemyData.colors[0], enemyData.colors[1]);

            Destroy(newEffect, 1f);
            Destroy(gameObject);
        }
    }

    IEnumerator Flash()
    {
        GetComponent<SpriteRenderer>().material = flashMat;
        yield return new WaitForSeconds(0.075f);
        GetComponent<SpriteRenderer>().material = mainMat;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + enemyData.attackOffset, enemyData != null ? enemyData.attackRange : 1f);
    }
}