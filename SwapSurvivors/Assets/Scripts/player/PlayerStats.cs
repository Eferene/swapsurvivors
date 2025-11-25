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
    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    private float _health;

    [Header("Combat")]
    [SerializeField] private float _damage = 50f;
    [SerializeField] private float _damageRange = 10f; // Hasar dalgalanma yüzdesi
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _attackRange = 1f;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;

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
        _health = _maxHealth;
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

    public float GiveDamage() // hasar verme
    {
        float multiplier = (Random.Range(-_damageRange, _damageRange) + 100) / 100;
        float currentDamage = _damage * multiplier; // Nihai hasar
        return currentDamage;
    }
}
