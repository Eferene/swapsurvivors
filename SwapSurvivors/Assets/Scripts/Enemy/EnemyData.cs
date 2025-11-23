using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/New Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public int baseHealth;
    public float speed;
    public GameObject enemyPrefab;

    [Header("Attack Info")]
    public float attackDamage;
    public float attackDamagePercentage = 10; // +- oranını belirler.
    public float attackRange;
    public float attackCooldown;

    [Header("Projectile Info")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
}
