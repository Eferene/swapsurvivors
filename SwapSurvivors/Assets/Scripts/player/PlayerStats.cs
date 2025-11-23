using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    // Singleton instance
    private static PlayerStats _instance = null;
    public static PlayerStats Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<PlayerStats>("PlayerStats"); // resources klasöründen asseti yükler
                _instance.Initialize(); // Başlangıç değerlerini ayarlar
            }
            return _instance;
        }
    }

    // Fields
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _damage = 50f;
    [SerializeField] private float _damageRange = 10f;
    [SerializeField] private float _speed = 5f;

    private float _health;

    // Properties
    public float PlayerHealth { get { return _health; } }
    public float PlayerMaxHealth { get { return _maxHealth; } }
    public float PlayerDamage { get { return _damage; } }
    public float PlayerSpeed { get { return _speed; } }

    public void Initialize()
    {
        _health = _maxHealth;
    }

    // --- Health management methods ---
    public void DecreaseHealth(float amount)
    {
        _health -= amount;
        if (_health < 0) _health = 0;
    }

    public void IncreaseHealth(float amount)
    {
        _health += amount;
        if (_health > _maxHealth) _health = _maxHealth;
    }

    // --- Damage management methods ---
    public void DecreaseDamage(float amount)
    {
        _damage -= amount;
        if (_damage < 0) _damage = 0;
    }

    public void IncreaseDamage(float amount)
    {
        _damage += amount;
    }

    public float GiveDamage()
    {
        float currentDamageRange = (Random.Range(-_damageRange, _damageRange) + 100) / 100;
        float currentDamage = _damage * currentDamageRange;
        return currentDamage;
    }
}
