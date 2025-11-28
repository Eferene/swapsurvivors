using UnityEngine;

public class ShotgunCharacter : BaseCharacterController
{
    [Header("Shotgun Özellikleri")]
    [SerializeField] private float cooldownPercantage = 0f;     // Shotgun için saldırı hızı çarpanı
    [SerializeField] private float healthMultiplier = 1.0f;     // Shotgun karakteri için hız
    [SerializeField] private float damageMultiplier = 1.0f;     // Shotgun için hasar çarpanı
    [SerializeField] private float rangeMultiplier = 5.0f;      // Shotgun için menzil çarpanı
    [SerializeField] private float speedMultiplier = 1.0f;      // Shotgun karakteri için hız çarpanı
    [SerializeField] private LayerMask enemyLayer;

    // Her çağrıldığında güncellenmesi için property olarak tanımlandı
    private float ShotgunCooldown => PlayerStats.Instance.AttackCooldown - (PlayerStats.Instance.AttackCooldown * (cooldownPercantage / 100));
    private float ShotgunMaxHealth => PlayerStats.Instance.PlayerMaxHealth * healthMultiplier;
    private float ShotgunDamage => PlayerStats.Instance.PlayerDamage * damageMultiplier;
    private float ShotgunRange => PlayerStats.Instance.AttackRange * rangeMultiplier;
    private float ShotgunSpeed => PlayerStats.Instance.PlayerSpeed * speedMultiplier;  // Shotgun karakteri için hız

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Attack()
    {
        Debug.Log("Shotgun Attack!");
    }

    protected override void FixedUpdate()
    {
        playerSpeed = ShotgunSpeed;
        base.FixedUpdate();
    }

    protected override float GetCooldown() => ShotgunCooldown;
}
