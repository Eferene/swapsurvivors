using UnityEngine;

public class ScyhteCharacter : BaseCharacterController
{
    [Header("Tırpan Özellikleri")]
    [SerializeField] private float cooldownMultiplier = 1.0f;   // Tırpan için saldırı hızı çarpanı
    [SerializeField] private float healthMultiplier = 1.0f;     // Tırpan karakteri için hız
    [SerializeField] private float damageMultiplier = 1.0f;     // Tırpan için hasar çarpanı
    [SerializeField] private float rangeMultiplier = 5.0f;      // Tırpan için menzil çarpanı
    [SerializeField] private float speedMultiplier = 1.0f;      // Tırpan karakteri için hız çarpanı
    [SerializeField] private float offset;                      // Tırpanın karakterden ne kadar uzakta başlayacağını belirler
    [SerializeField] private LayerMask enemyLayer;

    // Her çağrıldığında güncellenmesi için property olarak tanımlandı
    private float ScyhteCooldown => PlayerStats.Instance.AttackCooldown * cooldownMultiplier;
    private float ScyhteHealth => PlayerStats.Instance.PlayerMaxHealth * healthMultiplier;
    private float ScyhteDamage => PlayerStats.Instance.PlayerDamage * damageMultiplier;
    private float ScyhteRange => PlayerStats.Instance.AttackRange * rangeMultiplier;
    private float ScyhteSpeed => PlayerStats.Instance.PlayerSpeed * speedMultiplier;    // Tırpan karakteri için hız

    // --- Attack State ---
    private bool isRight = true;
    private Vector3 attackPos;

    // --- Visuals ---
    private GameObject semiCircle1;
    private GameObject semiCircle2;

    // --- Unity Methods ---
    protected override void Awake()
    {
        base.Awake();
        playerSpeed = ScyhteSpeed;
        semiCircle1 = transform.GetChild(0).gameObject;
        semiCircle2 = transform.GetChild(1).gameObject;
    }

    protected override void Attack()
    {
        attackPos = transform.position + new Vector3(isRight ? offset : -offset, 0f, 0f);

        // Yakındaki düşmanları bul
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPos, ScyhteRange, enemyLayer);

        foreach (var enemy in enemies)
        {
            Vector2 dir = enemy.transform.position - transform.position;
            dir.Normalize();

            bool isEnemyOnRight = Vector2.Dot(dir, transform.right) > 0;

            if (isRight == isEnemyOnRight)
                ApplyDamage(enemy, PlayerStats.Instance.GiveDamage(ScyhteDamage));
        }

        if (isRight)
        {
            semiCircle1.SetActive(true);
            semiCircle2.SetActive(false);
        }
        else
        {
            semiCircle1.SetActive(false);
            semiCircle2.SetActive(true);
        }

        isRight = !isRight;
    }

    protected override float GetCooldown() => ScyhteCooldown;

    void ApplyDamage(Collider2D enemy, float damage)
    {
        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.TakeDamage(damage);
            Debug.Log($"{enemy.name} gelen {damage} hasarı yedi.");
        }
    }

    protected override void FixedUpdate()
    {
        playerSpeed = ScyhteSpeed;
        base.FixedUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        // Sağ taraf vurulacaksa mavi, sol taraf vurulacaksa kırmızı çizelim.
        Gizmos.color = Color.red;

        // Karakterin yönü
        Vector3 rightDir = transform.right * ScyhteRange;
        Vector3 leftDir = -transform.right * ScyhteRange;


        Gizmos.DrawLine(attackPos, attackPos + (isRight ? leftDir : rightDir));

    }
}