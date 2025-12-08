using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScyhteCharacter : BaseCharacterController
{
    [SerializeField] private float offset;                      // Tırpanın karakterden ne kadar uzakta başlayacağını belirler
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visuals")]
    //[SerializeField] private Animator scytheAnimator; // Tırpanın Animatorü
    [SerializeField] private GameObject scythePrefab;       // Tırpan objesi
    [SerializeField] private Transform staticScythe;  // Seviye 3 için dönen tırpan objesi

    [Header("Level 2 Timing")]
    [SerializeField] private float attackAnimationDuration = 0.5f; // Vuruş animasyon süresi

    Vector3 scytheScale;

    // --- Fieldlar ---
    private bool isRight = true;
    private Vector3 attackPos;
    private List<Collider2D> hitBuffer = new List<Collider2D>();
    private List<GameObject> scythePool = new List<GameObject>(); // Tırpan havuzu
    ContactFilter2D filter = new ContactFilter2D();

    // --- Unity Methods ---
    protected override float GetCooldown() => playerManager.CurrentCooldown;

    protected override void Awake()
    {
        base.Awake();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
    }

    private void Start()
    {
        scytheScale = new Vector3(playerManager.CurrentRange, playerManager.CurrentRange);
    }

    protected override void Update()
    {
        base.Update();
        if (playerManager.CharacterLevel == 3)
            LevelThreeAttack();
    }

    protected override void Attack()
    {
        switch (playerManager.CharacterLevel)
        {
            case 1: // level 1
                staticScythe.gameObject.SetActive(false);
                LevelOneAttack();
                break;
            case 2: // level 2
                staticScythe.gameObject.SetActive(false);
                LevelTwoAttack();
                break;
            case 3: // level 3
                break;
            default: // default level 1
                staticScythe.gameObject.SetActive(false);
                LevelOneAttack();
                break;
        }
    }

    private GameObject GetScytheFromPool()
    {
        // Havuzu tara
        foreach (GameObject obj in scythePool)
        {
            // Obje kullanılmıyorsa
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Hepsi kullanılıyorsa yeni obje oluştur
        GameObject newScythe = Instantiate(scythePrefab, transform);
        scythePool.Add(newScythe);
        return newScythe;
    }

    private IEnumerator ReturnToPoolDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private GameObject SpawnScythe(bool isRight)
    {
        GameObject scytheInstance = GetScytheFromPool();

        // Tırpan pozisyonu
        float currentOffset = isRight ? offset + 1 : -offset - 1;
        Vector3 scythePos = transform.position + new Vector3(currentOffset, 0f, 0f);
        scytheInstance.transform.position = scythePos;

        // Scale
        Vector3 tempScale = scytheScale;
        tempScale.x = isRight ? Mathf.Abs(tempScale.x) : -Mathf.Abs(tempScale.x);
        scytheInstance.transform.localScale = tempScale;

        // Animator
        Animator animator = scytheInstance.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind(); // Animatörü sıfırla
            animator.Update(0f); // İlk kareyi uygula
            animator.SetTrigger("Attack");
        }

        return scytheInstance;
    }

    public void DoScytheHit(bool attackRight)
    {
        // Saldırı pozisyonu
        float currentOffset = attackRight ? offset : -offset;
        attackPos = transform.position + new Vector3(currentOffset, 0f, 0f);

        float currentRange = playerManager.CurrentRange;
        // Yakındaki düşmanları bulur
        int hitCount = Physics2D.OverlapCircle(attackPos, currentRange, filter, hitBuffer);

        for (int i = 0; i < hitCount; i++)
        {
            int currentDamage = playerManager.GiveDamageCharacter();

            var enemy = hitBuffer[i];
            if (enemy == null) continue;

            // Düşmanın karaktere göre konumunu hesaplar
            Vector2 dir = enemy.transform.position - transform.position;

            // Düşmanın karakterin sağında mı solunda mı?
            bool isEnemyOnRight = Vector2.Dot(dir, transform.right) > 0;

            // Sadece tırpanın vurduğu taraftaki düşmanlara hasar uygular
            if (attackRight == isEnemyOnRight)
                ApplyDamage(enemy, currentDamage);
        }
    }


    private void LevelOneAttack()
    {
        DoScytheHit(isRight);
        GameObject scytheInstance = SpawnScythe(isRight);
        StartCoroutine(ReturnToPoolDelayed(scytheInstance, attackAnimationDuration));
        isRight = !isRight;

        //StartCoroutine(DeactivateScytheAfterDelay(attackAnimationDuration));
    }

    private void LevelTwoAttack()
    {
        // Yeni combo başlat
        StartCoroutine(LevelTwoComboRoutine());
    }

    private IEnumerator LevelTwoComboRoutine()
    {
        // 1. Sağ vuruş
        DoScytheHit(true);
        GameObject rightScythe = SpawnScythe(true);
        StartCoroutine(ReturnToPoolDelayed(rightScythe, attackAnimationDuration));

        // Animasyon süresini bekle
        yield return new WaitForSeconds(attackAnimationDuration);

        // 2. Sol vuruş
        DoScytheHit(false);
        GameObject leftScythe = SpawnScythe(false);
        StartCoroutine(ReturnToPoolDelayed(leftScythe, attackAnimationDuration));
    }

    private void LevelThreeAttack()
    {
        staticScythe.gameObject.SetActive(true);
        staticScythe.RotateAround(transform.position, Vector3.forward, -(360f / playerManager.CurrentCooldown) * Time.deltaTime);
    }

    void ApplyDamage(Collider2D enemy, float damage)
    {
        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.TakeDamage(damage);
            //Debug.Log($"{enemy.name} gelen {damage} hasarı yedi.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerManager == null)
            return;

        // Sağ taraf vurulacaksa mavi, sol taraf vurulacaksa kırmızı çizelim.
        Gizmos.color = Color.red;

        // Karakterin yönü
        Vector3 rightDir = transform.right * playerManager.CurrentRange;
        Vector3 leftDir = -transform.right * playerManager.CurrentRange;


        Gizmos.DrawLine(attackPos, attackPos + (isRight ? leftDir : rightDir));

    }
}