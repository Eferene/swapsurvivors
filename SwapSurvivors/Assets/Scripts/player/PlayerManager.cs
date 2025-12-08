using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats baseStats;

    // --- Runtime Stats ---
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
    public float CurrentDamage { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float CurrentCooldown { get; private set; }
    public float CurrentRange { get; private set; }

    // --- Level & Score ---
    public int CharacterLevel { get; private set; } = 1;
    public int Score { get; private set; }

    // --- Multipliers ---
    private float _damageMult = 1f;
    private float _speedMult = 1f;
    private float _rangeMult = 1f;
    private float _healthMult = 1f;
    private float _cooldownReductionPerc = 0f;

    // --- Events (UI) ---
    public event Action<float, float> OnHealthChanged; // (Current, Max) gönderir
    public event Action<int> OnScoreChanged; // (New Score) gönderir

    private void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        // SO'dan verileri çekip runtime değişkenlere yazıyoruz
        MaxHealth = baseStats.BaseMaxHealth;
        CurrentHealth = MaxHealth;
        CurrentDamage = baseStats.BaseDamage;
        CurrentSpeed = baseStats.BaseSpeed;
        CurrentCooldown = baseStats.BaseAttackCooldown;
        CurrentRange = baseStats.BaseRange;

        Debug.Log("PlayerManager: Statlar yüklendi");
    }

    // --- Health Management Methods ---
    public void TakeDamageCharacter(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;

        // UI'a haber
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("Die");
        }
    }

    public void HealCharacter(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    // --- Damage Management Methods ---
    public int GiveDamageCharacter()
    {
        // Silah çarpanı * Base Damage * Upgrade Çarpanı
        float rawDamage = CurrentDamage * _damageMult;

        // Randomize
        float fluctuation = baseStats.DamageRangePercentage;
        float multiplier = (UnityEngine.Random.Range(-fluctuation, fluctuation) + 100) / 100;

        return Mathf.RoundToInt(rawDamage * multiplier);
    }

    // --- Upgrade Management Methods ---
    public void ApplyUpgrades(float damageMultiplier, float rangeMultiplier, float cooldownPercentage, float speedMultiplier, float healthMultiplier)
    {
        _damageMult *= damageMultiplier;
        _rangeMult *= rangeMultiplier;
        _cooldownReductionPerc += cooldownPercentage;
        _speedMult *= speedMultiplier;
        _healthMult *= healthMultiplier;

        CurrentDamage = CurrentDamage * damageMultiplier;
        CurrentRange = CurrentRange * rangeMultiplier;
        CurrentSpeed = CurrentSpeed * speedMultiplier;
        MaxHealth = MaxHealth * _healthMult;
    }

    // --- Score and Level Management Methods ---
    public void AddScore(int amount)
    {
        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }

    public void IncreaseCharacterLevel(int amount)
    {
        if (CharacterLevel < 3)
            CharacterLevel += amount;
        Debug.Log("Character Level Increased to: " + CharacterLevel);
    }
}