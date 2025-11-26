using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    // --- Singleton ---
    private static PlayerStats _instance = null;
    public static PlayerStats Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<PlayerStats>("PlayerStats"); // Resources klasöründen PlayerStats assetini yükler
                _instance.Initialize(); // Başlangıç değerlerini ayarlar
            }
            return _instance;
        }
    }

    // --- Fields ---
    [Header("Base Stats")]
    [SerializeField] private float _baseMaxHealth = 100f;
    [SerializeField] private float _baseDamage = 50f;
    [SerializeField] private float _baseAttackCooldown = 1f;
    [SerializeField] private float _baseAttackRange = 1f;
    [SerializeField] private float _baseSpeed = 5f;

    private float _maxHealth;
    private float _health;
    private float _damage;
    private float _attackCooldown;
    private float _attackRange;
    private float _speed;

    [Header("Combat")]
    [SerializeField] private float _damageRange = 10f; // Hasar dalgalanma yüzdesi

    [Header("Upgrades")]
    [SerializeField] private float upgradesCooldownMultiplier = 1.0f;
    [SerializeField] private float upgradesDamageMultiplier = 1.0f;
    [SerializeField] private float upgradesRangeMultiplier = 1.0f;
    [SerializeField] private float upgradesSpeedMultiplier = 1.0f;
    [SerializeField] private float upgradesHealthMultiplier = 1.0f;

    [Header("Stats")]
    [SerializeField] private int _level = 1;
    [SerializeField] private int _score = 0;
    [SerializeField] private int _wave = 0;

    // --- Properties ---
    public float PlayerHealth => _health; // Yalnızca get için lambda ifadesi kullanılabilir
    public float PlayerMaxHealth => _maxHealth;
    public float PlayerDamage => _damage;
    public float PlayerSpeed => _speed;
    public float AttackCooldown => _attackCooldown;
    public float AttackRange => _attackRange;
    public int PlayerLevel => _level;
    public int PlayerScore => _score;
    public int CurrentWave => _wave;

    // --- Initialization ---
    public void Initialize()
    {
        _maxHealth = _baseMaxHealth;
        _health = _maxHealth;
        _damage = _baseDamage;
        _attackCooldown = _baseAttackCooldown;
        _attackRange = _baseAttackRange;
        _speed = _baseSpeed;
    }

    // --- Health Management Methods ---
    public void DecreaseHealth(float amount) // hasar alma
    {
        _health -= amount;
        if (_health < 0) _health = 0;
    }

    public void IncreaseHealth(float amount) // can yenileme
    {
        _health += amount;
        if (_health > _maxHealth) _health = _maxHealth;
    }

    // --- Damage Management Methods ---
    public void DecreaseDamage(float amount) // hasar düşürme
    {
        _damage -= amount;
        if (_damage < 0) _damage = 0;
    }

    public void IncreaseDamage(float amount) // hasar artırma
    {
        _damage += amount;
    }

    public float GiveDamage(float damage) // hasar verme
    {
        float multiplier = (Random.Range(-_damageRange, _damageRange) + 100) / 100;
        float currentDamage = damage * multiplier; // Nihai hasar
        return currentDamage;
    }

    // --- Upgrade Management Methods ---
    public void ApplyUpgrades(float damageMultiplier, float rangeMultiplier, float cooldownMultiplier, float speedMultiplier)
    {
        upgradesDamageMultiplier *= damageMultiplier;
        upgradesRangeMultiplier *= rangeMultiplier;
        upgradesCooldownMultiplier *= cooldownMultiplier;
        upgradesSpeedMultiplier *= speedMultiplier;

        _damage = _baseDamage * upgradesDamageMultiplier;
        _attackRange = _baseAttackRange * upgradesRangeMultiplier;
        _attackCooldown = _baseAttackCooldown / upgradesCooldownMultiplier;
        _speed = _baseSpeed * upgradesSpeedMultiplier;
        _maxHealth = _baseMaxHealth * upgradesHealthMultiplier;
    }

    // --- Score and Level Management Methods ---
    public void AddScore(int amount)
    {
        _score += amount;
    }
}
