using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/New Enemy")]
public class EnemyData : ScriptableObject
{
    private bool MeleeEnemy() => enemyType == EnemyType.Melee;
    private bool ProjectileEnemy() => enemyType == EnemyType.Projectile;
    private bool ThrowEnemy() => enemyType == EnemyType.Throw;
    private bool RaycastEnemy() => enemyType == EnemyType.Raycast;

    [Header("Basic Info")]
    public string enemyName;
    public int baseHealth;
    public float speed;
    public GameObject enemyPrefab;
    public EnemyType enemyType;

    [Header("Attack Info")]
    public int attackDamage;
    public float attackDamagePercentage = 10; // +- oranını belirler.
    public float attackRange;
    public float attackCooldown;
    public Vector3 attackOffset;

    [Header("Projectile Info")]
    [ShowIf("ProjectileEnemy")] public GameObject projectilePrefab;
    [ShowIf("ProjectileEnemy")] public float projectileSpeed;

    [Header("Throw Info")] // Throw attackalr için damage, cooldown Attack Info kısmında belirtilir.
    [ShowIf("ThrowEnemy")] public GameObject throwPrefab;
    [ShowIf("ThrowEnemy")] public float throwRadius = 5f;
    [ShowIf("ThrowEnemy")] public int throwCount = 3;
    [ShowIf("ThrowEnemy")] public float damageRadius = 1f;
    [ShowIf("ThrowEnemy")] public ThrowDamageType throwDamageType;
    [ShowIf("ThrowEnemy")] public float damageOverTimeDuration = 2f;
    [ShowIf("ThrowEnemy")] public float overTimeDamageInterval = 0.1f;

    [Header("Stats Values")]
    public int scoreGain;
    public float scoreGainPercentage = 10; // +- oranını belirler.

    [Header("Effects")]
    public Color[] colors = new Color[2];
}

public enum ThrowDamageType
{
    Instant,
    OverTime
}

public enum EnemyType
{
    Melee,
    Projectile,
    Raycast,
    Throw
}