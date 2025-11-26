using System.Security.Cryptography.X509Certificates;
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
    [SerializeField] private const float BaseAttackCooldown = 1f;
    [SerializeField] private const float BaseMaxHealth = 100f;
    [SerializeField] private const float BaseDamage = 50f;
    [SerializeField] private const float BaseRange = 1f;
    [SerializeField] private const float BaseSpeed = 5f;

    private float _attackCooldown;
    private float _maxHealth;
    private float _health;
    private float _damage;
    private float _range;
    private float _speed;
    private float _characterLevel;

    [Header("Combat")]
    [SerializeField] private const float _damageRange = 10f; // Hasar dalgalanma yüzdesi

    [Header("Upgrades")]
    [SerializeField] private float _upgradesCooldownMultiplier = 1.0f;
    [SerializeField] private float _upgradesHealthMultiplier = 1.0f;
    [SerializeField] private float _upgradesDamageMultiplier = 1.0f;
    [SerializeField] private float _upgradesRangeMultiplier = 1.0f;
    [SerializeField] private float _upgradesSpeedMultiplier = 1.0f;

    [Header("Stats")]
    [SerializeField] private int _score = 0;
    [SerializeField] private int _wave = 1;

    // --- Properties ---
    public float AttackCooldown => _attackCooldown;
    public float CharacterLevel => _characterLevel;
    public float PlayerMaxHealth => _maxHealth;
    public float PlayerHealth => _health; // Yalnızca get için lambda ifadesi kullanılabilir
    public float PlayerDamage => _damage;
    public float PlayerSpeed => _speed;
    public float AttackRange => _range;
    public int PlayerScore => _score;
    public int CurrentWave => _wave;

    // --- Initialization ---
    public void Initialize()
    {
        _maxHealth = BaseMaxHealth;
        _health = _maxHealth;
        _damage = BaseDamage;
        _attackCooldown = BaseAttackCooldown;
        _range = BaseRange;
        _speed = BaseSpeed;

        _upgradesCooldownMultiplier = 1.0f;
        _upgradesHealthMultiplier = 1.0f;
        _upgradesDamageMultiplier = 1.0f;
        _upgradesRangeMultiplier = 1.0f;
        _upgradesSpeedMultiplier = 1.0f;

        _characterLevel = 1;
        _score = 0;
        _wave = 1;
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
        _upgradesDamageMultiplier *= damageMultiplier;
        _upgradesRangeMultiplier *= rangeMultiplier;
        _upgradesCooldownMultiplier *= cooldownMultiplier;
        _upgradesSpeedMultiplier *= speedMultiplier;

        _damage = BaseDamage * _upgradesDamageMultiplier;
        _range = BaseRange * _upgradesRangeMultiplier;
        _attackCooldown = BaseAttackCooldown / _upgradesCooldownMultiplier;
        _speed = BaseSpeed * _upgradesSpeedMultiplier;
        _maxHealth = BaseMaxHealth * _upgradesHealthMultiplier;
    }

    // --- Score and Level Management Methods ---
    public void AddScore(int amount)
    {
        _score += amount;
    }
}
