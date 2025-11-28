using UnityEngine;

public class ScyhteCharacter : BaseCharacterController
{
    [Header("Tırpan Özellikleri")]
    [SerializeField] private float cooldownPercantage = 0f;     // Tırpan için saldırı hızı yüzdesi
    [SerializeField] private float healthMultiplier = 1.0f;     // Tırpan karakteri için hız
    [SerializeField] private float damageMultiplier = 1.0f;     // Tırpan için hasar çarpanı
    [SerializeField] private float rangeMultiplier = 5.0f;      // Tırpan için menzil çarpanı
    [SerializeField] private float speedMultiplier = 1.0f;      // Tırpan karakteri için hız çarpanı
    [SerializeField] private float offset;                      // Tırpanın karakterden ne kadar uzakta başlayacağını belirler
    [SerializeField] private LayerMask enemyLayer;

    // Her çağrıldığında güncellenmesi için property olarak tanımlandı
    private float ScytheCooldown => PlayerStats.Instance.AttackCooldown - (PlayerStats.Instance.AttackCooldown * (cooldownPercantage / 100));
    private float ScytheMaxHealth => PlayerStats.Instance.PlayerMaxHealth * healthMultiplier;
    private float ScytheDamage => PlayerStats.Instance.PlayerDamage * damageMultiplier;
    private float ScytheRange => PlayerStats.Instance.AttackRange * rangeMultiplier;
    private float ScytheSpeed => PlayerStats.Instance.PlayerSpeed * speedMultiplier;    // Tırpan karakteri için hız

    // --- Attack State ---
    private bool isRight = true;
    private Vector3 attackPos;

    // --- Visuals ---
    [Header("Visuals")]
    [SerializeField] private Animator scytheAnimator; // Tırpanın Animator'ını buraya sürükle
    [SerializeField] private GameObject scytheTransform; // Tırpan objesinin Transform'u (yön çevirmek için)
    Vector3 scytheScale;

    // --- Unity Methods ---
    protected override void Awake()
    {
        base.Awake();
        playerSpeed = ScytheSpeed;
    }

    protected override void Update()
    {
        Debug.Log(PlayerStats.Instance.AttackCooldown + " " + ScytheCooldown);
        base.Update();
    }

    protected override void Attack()
    {
        switch (PlayerStats.Instance.CharacterLevel)
        {
            case 1:
                LevelOneAttack();
                break;
            case 2:
                LevelTwoAttack();
                break;
            case 3:
                LevelThreeAttack();
                break;
            default:
                LevelOneAttack();
                break;
        }
    }

    private void LevelOneAttack()
    {
        float currentOffset = isRight ? offset : -offset;

        attackPos = transform.position + new Vector3(currentOffset, 0f, 0f);

        // Scythe animation yönünü ayarlar
        scytheTransform.transform.position = attackPos;

        scytheScale = new Vector3(ScytheRange, ScytheRange);
        scytheScale.x = isRight ? Mathf.Abs(scytheScale.x) : -Mathf.Abs(scytheScale.x);
        scytheTransform.transform.localScale = scytheScale;

        scytheTransform.gameObject.SetActive(true);
        scytheAnimator.SetTrigger("Attack");

        // Yakındaki düşmanları bulur
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

    private void LevelTwoAttack()
    {

    }

    private void LevelThreeAttack()
    {
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