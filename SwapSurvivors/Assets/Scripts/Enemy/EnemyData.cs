using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/New Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int baseHealth;
    public float speed;
    public GameObject enemyPrefab;

    // Attack values
    public float attackDamage;
    public float attackDamagePercentage = 10; // +- oranını belirler.
    public float attackRange;
    public float attackCooldown;
}
