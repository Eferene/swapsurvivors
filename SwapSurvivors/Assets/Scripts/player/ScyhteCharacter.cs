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
    private float ScytheCooldown => PlayerStats.Instance.AttackCooldown * cooldownMultiplier;
    private float ScytheHealth => PlayerStats.Instance.PlayerMaxHealth * healthMultiplier;
    private float ScytheDamage => PlayerStats.Instance.PlayerDamage * damageMultiplier;
    private float ScytheRange => PlayerStats.Instance.AttackRange * rangeMultiplier;
    private float ScytheSpeed => PlayerStats.Instance.PlayerSpeed * speedMultiplier;    // Tırpan karakteri için hız

    // --- Attack State ---
    private bool isRight = true;
    private Vector3 attackPos;

    // --- Visuals ---
    [Header("Visuals")]
    [SerializeField] private Animator scytheAnimator; // Tırpanın Animator'ını buraya sürükle
    [SerializeField] private Transform scytheVisualTransform; // Tırpan objesinin Transform'u (yön çevirmek için)

    // --- Unity Methods ---
    protected override void Awake()
    {
        base.Awake();
        playerSpeed = ScytheSpeed;
    }

    protected override void Attack()
    {
        float currentOffset = isRight ? offset : -offset;

        attackPos = transform.position + new Vector3(currentOffset, 0f, 0f);

        // Tırpan görselini yönlendir
        Vector3 visualScale = scytheVisualTransform.localScale;
        visualScale.x = isRight ? Mathf.Abs(visualScale.x) : -Mathf.Abs(visualScale.x);
        scytheVisualTransform.localScale = visualScale;

        scytheVisualTransform.position = attackPos;
        Debug.Log(attackPos);

        scytheAnimator.SetTrigger("Attack");

        // Yakındaki düşmanları bul
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPos, ScytheRange, enemyLayer);

        foreach (var enemy in enemies)
        {
            Vector2 dir = enemy.transform.position - transform.position;
            dir.Normalize();

            bool isEnemyOnRight = Vector2.Dot(dir, transform.right) > 0;

            if (isRight == isEnemyOnRight)
                ApplyDamage(enemy, PlayerStats.Instance.GiveDamage(ScytheDamage));
        }

        isRight = !isRight;
    }

    protected override float GetCooldown() => ScytheCooldown;

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
        playerSpeed = ScytheSpeed;
        base.FixedUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        // Sağ taraf vurulacaksa mavi, sol taraf vurulacaksa kırmızı çizelim.
        Gizmos.color = Color.red;

        // Karakterin yönü
        Vector3 rightDir = transform.right * ScytheRange;
        Vector3 leftDir = -transform.right * ScytheRange;


        Gizmos.DrawLine(attackPos, attackPos + (isRight ? leftDir : rightDir));

    }
}