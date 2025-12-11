using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    #region Variables

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileCount = 6;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Bounce Settings (Level 3)")]
    [SerializeField] private float bounceRange = 5.0f;
    [SerializeField] private int maxBounceCount = 5;
    [SerializeField] private float bounceSpeedMultiplier = 1.5f;

    // Stats
    private float boomerangRange;
    private float boomerangSpeed;
    private int characterLevel;

    // Internal States
    private Vector3 startPosition;
    private bool isReturning = false;
    private bool isBouncing = false;
    private int currentBounceCount = 0;
    private Transform currentBounceTarget;
    private List<GameObject> hitEnemiesInChain = new List<GameObject>(); // Aynı turda vurulanları tutar

    // References
    private GameObject ownerObject;
    private Rigidbody2D rb;
    private PlayerManager playerManager;

    #endregion

    #region Unity Methods

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime); // Bumerang dönme

        if (isReturning) ReturnToPlayer();
        else if (isBouncing) HandleBounceMovement();
        else
        {
            if (Vector3.Distance(startPosition, transform.position) >= boomerangRange)
                OnReachMaxRange();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Düşmanla çarpışma kontrolü
        if (collision.CompareTag("Enemy"))
        {
            // Eğer manuel hit zaten bu işi yaptıysa tekrar yapma!
            if (hitEnemiesInChain.Contains(collision.gameObject)) return;
            if (collision.TryGetComponent(out EnemyController enemyController))
            {
                if (enemyController.IsDead) return;
                float boomerangDamage = playerManager.GiveDamageCharacter();
                enemyController.TakeDamage(boomerangDamage);

                if (isBouncing)
                {
                    // Vurduğumuz düşmanı listeye ekle
                    hitEnemiesInChain.Add(collision.gameObject);
                    currentBounceCount++;

                    // Yeni hedef ara
                    if (!FindNextBounceTarget())
                        StartReturn(); // Başka hedef yoksa geri dön
                }
            }
        }
    }
    #endregion

    #region Setup
    // Bu metod, bumerang özelliklerini ayarlamak için çağrılır
    public void Setup(PlayerManager manager, float range, float speed, int level, GameObject owner)
    {
        playerManager = manager;
        boomerangRange = range;
        boomerangSpeed = speed;
        characterLevel = level;
        ownerObject = owner;

        startPosition = transform.position;
        isReturning = false;
        isBouncing = false;
        currentBounceCount = 0;
        currentBounceTarget = null;
        hitEnemiesInChain.Clear();

        rb.linearVelocity = transform.right * speed;
    }
    #endregion

    #region Helper Logic

    private void OnReachMaxRange()
    {
        // Level 2 ve üzeri için Projectile fırlat
        if (characterLevel >= 2)
            SpawnProjectiles();
        // Level 3 ise Sekme olayına gir, değilse direkt dön
        if (characterLevel >= 3)
            TryStartBouncing();
        else
            StartReturn();
    }

    private void TryStartBouncing()
    {
        // İlk hedefi bulmayı dene
        if (FindNextBounceTarget())
        {
            isBouncing = true;
            currentBounceCount = 0;
        }
        else StartReturn(); // Etrafta düşman yoksa direkt dön
    }

    private bool FindNextBounceTarget()
    {
        // Eğer maksimum sekme sayısına ulaştıysak dön
        if (currentBounceCount >= maxBounceCount) return false;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bounceRange);
        float closestDist = Mathf.Infinity;
        Transform potentialTarget = null;

        foreach (var collider in enemies)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Vurulmuş düşmanı pas geç
                if (hitEnemiesInChain.Contains(collider.gameObject))
                    continue;

                // Ölü düşmanı pas geç 
                if (collider.TryGetComponent(out EnemyController enemy) && enemy.IsDead)
                    continue;

                float dist = Vector2.Distance(transform.position, collider.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    potentialTarget = collider.transform;
                }
            }
        }

        if (potentialTarget != null)
        {
            currentBounceTarget = potentialTarget;
            return true;
        }

        return false;
    }

    private void HandleBounceMovement()
    {
        if (currentBounceTarget == null)
        {
            // Hedef yolda öldüyse veya kaybolduysa yenisini ara veya dön
            if (!FindNextBounceTarget()) StartReturn();
            return;
        }

        Vector3 dir = (currentBounceTarget.position - transform.position).normalized;
        rb.linearVelocity = dir * (boomerangSpeed * bounceSpeedMultiplier);

        // --- OVERSHOOT KORUMASI ---
        // Eğer hedefe çok yaklaştık ama OnTriggerEnter hala çalışmadıysa,
        // manuel olarak vurduk sayıp devam ediyoruz.
        float distanceToTarget = Vector2.Distance(transform.position, currentBounceTarget.position);

        if (distanceToTarget < 0.5f)
            ManualHit(currentBounceTarget.gameObject);
    }

    // Fizik motoru kaçırırsa biz devreye gireriz   
    private void ManualHit(GameObject enemyObj)
    {

        // Zaten vurduklarımız arasındaysa işlem yapma (Çifte hasarı önle)
        if (hitEnemiesInChain.Contains(enemyObj)) return;
        if (enemyObj.TryGetComponent(out EnemyController enemyController))
        {
            if (enemyController.IsDead) return;

            // Hasarı ver
            if (playerManager != null)
                enemyController.TakeDamage(playerManager.GiveDamageCharacter());

            // Listeye ekle
            hitEnemiesInChain.Add(enemyObj);
            currentBounceCount++;

            // Yeni hedef ara
            if (!FindNextBounceTarget())
                StartReturn();
            Debug.Log("Manuel Hit Çalıştı");
        }
    }

    private void StartReturn()
    {
        isBouncing = false;
        isReturning = true;
        currentBounceTarget = null;
    }

    private void ReturnToPlayer()
    {
        Vector3 directionToPlayer = (ownerObject.transform.position - transform.position).normalized;
        rb.linearVelocity = directionToPlayer * boomerangSpeed;

        // Sadece geri dönüyorsa mesafeyi kontrol et
        if (Vector2.Distance(ownerObject.transform.position, transform.position) < 0.5f)
            ReturnToPool(gameObject);
    }

    private void ReturnToPool(GameObject obj)
    {
        rb.linearVelocity = Vector2.zero;
        isReturning = false;
        obj.SetActive(false);
    }

    private void SpawnProjectiles()
    {
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = i * angleStep;

            float dirX = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            Vector3 spawnDirection = new Vector3(dirX, dirY, 0);

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            if (projectile.TryGetComponent(out ProjectileController script))
                script.Setup(playerManager.GiveDamageCharacter() / 2, boomerangSpeed, spawnDirection);
        }
    }
    #endregion
}
