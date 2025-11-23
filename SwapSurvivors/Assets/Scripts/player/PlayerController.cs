using UnityEngine;

public class PlayerController : BaseCharacterController
{
    [Header("Tırpan Özellikleri")]
    [SerializeField] private float scyhteRange = 5f;
    [SerializeField] private LayerMask enemyLayer;

    protected override void Awake()
    {
        base.Awake();
        base.attackCooldown = 1.0f; // Tırpan için saldırı hızı
    }

    protected override void Attack()
    {
        Debug.Log("Tırpan vurdu!");

        // Yakındaki düşmanları bul
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            scyhteRange,
            enemyLayer
        );

        foreach (var dusman in enemies)
        {
            float hasar = PlayerStats.Instance.GiveDamage();
            Debug.Log($"{dusman.name} tırpandan {hasar} hasar aldı!");

            enemyController = dusman.GetComponent<EnemyController>();
            enemyController.TakeDamage(hasar);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scyhteRange);
    }
}