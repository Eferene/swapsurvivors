using System;
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
    [SerializeField] private float BaseAttackCooldown = 1f;
    [SerializeField] private float BaseMaxHealth = 100f;
    [SerializeField] private float BaseDamage = 50f;
    [SerializeField] private float BaseRange = 1f;
    [SerializeField] private float BaseSpeed = 5f;

    private float _attackCooldown;
    private float _maxHealth;
    private float _baseHealth;
    private float _health;
    private float _damage;
    private float _range;
    private float _speed;

    private float _characterLevel;

    [Header("Combat")]
    [SerializeField] private float _damageRange = 10f; // Hasar dalgalanma yüzdesi

    [Header("Upgrades")]
    [SerializeField] private float _upgradesCooldownPercantage = 0f;
    [SerializeField] private float _upgradesHealthMultiplier = 1.0f;
    [SerializeField] private float _upgradesDamageMultiplier = 1.0f;
    [SerializeField] private float _upgradesRangeMultiplier = 1.0f;
    [SerializeField] private float _upgradesSpeedMultiplier = 1.0f;

    [Header("Stats")]
    [SerializeField] private int _score = 0;
    [SerializeField] private int _wave = 1;

    // --- Properties ---
    public float AttackCooldown => _attackCooldown; // Yalnızca get için lambda ifadesi kullanılabilir
    public float CharacterLevel => _characterLevel;
    public float PlayerMaxHealth => _maxHealth;
    public float PlayerHealth => _health;
    public float PlayerDamage => _damage;
    public float PlayerSpeed => _speed;
    public float AttackRange => _range;
    public int PlayerScore => _score;
    public int CurrentWave => _wave;

    // --- Initialization ---
    public void Initialize()
    {
        Debug.Log("PlayerStats Initialized");

        _maxHealth = BaseMaxHealth;
        _baseHealth = BaseMaxHealth;
        _health = _maxHealth;
        _damage = BaseDamage;
        _attackCooldown = BaseAttackCooldown;
        _range = BaseRange;
        _speed = BaseSpeed;

        _upgradesCooldownPercantage = 0f;
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
        UIController.Instance.UpdateHealthSlider();
    }

    public void IncreaseHealth(float amount) // can yenileme
    {
        _health += amount;
        if (_health > _maxHealth) _health = _maxHealth;
        UIController.Instance.UpdateHealthSlider();
    }

    // --- Damage Management Methods ---
    public void DecreaseDamageMultiplier(float amount) // hasar düşürme
    {
        _upgradesDamageMultiplier -= amount;
    }

    public void IncreaseDamage(float amount) // hasar artırma
    {
        _upgradesDamageMultiplier += amount;
    }

    public int GiveDamage(float damage) // hasar verme
    {
        float multiplier = (UnityEngine.Random.Range(-_damageRange, _damageRange) + 100) / 100;
        int currentDamage = Convert.ToInt32(damage * multiplier); // Nihai hasar
        return currentDamage;
    }

    // --- Upgrade Management Methods ---
    public void ApplyUpgrades(float damageMultiplier, float rangeMultiplier, float cooldownPercentage, float speedMultiplier)
    {
        _upgradesDamageMultiplier *= damageMultiplier;
        _upgradesRangeMultiplier *= rangeMultiplier;
        _upgradesCooldownPercantage += cooldownPercentage;
        _upgradesSpeedMultiplier *= speedMultiplier;

        _damage = BaseDamage * _upgradesDamageMultiplier;
        _range = BaseRange * _upgradesRangeMultiplier;
        _attackCooldown = BaseAttackCooldown - (BaseAttackCooldown * (_upgradesCooldownPercantage / 100));
        _speed = BaseSpeed * _upgradesSpeedMultiplier;
        _maxHealth = BaseMaxHealth * _upgradesHealthMultiplier;
    }

    // --- Score and Level Management Methods ---
    public void AddScore(int amount)
    {
        _score += amount;
    }

    public void IncreaseCharacterLevel(int amount)
    {
        if (CharacterLevel < 4)
            _characterLevel += amount;
        Debug.Log("Character Level Increased to: " + _characterLevel);
    }

    // --- Auto Reset Logic ---
    // Bu kod oyun her "Play" tuşuna basıldığında otomatik çalışır.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void ResetStatics()
    {
        _instance = null;
    }
}
