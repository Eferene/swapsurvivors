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

    // --- Upgrade Stats ---
    public float DamageMultiplier { get; private set; } = 1f;
    public float SpeedMultiplier { get; private set; } = 1f;
    public float RangeMultiplier { get; private set; } = 1f;
    public float MaxHealthBonus { get; private set; } = 1f;
    public float HealthRegenBonus { get; private set; } = 1f;
    public float LifeStealPercentage { get; private set; } = 0f;
    public float AttackSpeedPercentage { get; private set; } = 1f;
    public float CriticalHitChance { get; private set; } = 0f;
    public float CriticalHitDamageMultiplier { get; private set; } = 2f;


    // --- Level & Score ---
    public int CharacterLevel { get; private set; } = 3;
    public int Score { get; private set; }

    // --- Events (UI) ---
    public event Action<float, float> OnHealthChanged; // (Current, Max) gönderir
    public event Action<int> OnScoreChanged; // (New Score) gönderir
    public event Action<int> OnLevelChanged; // (New Level) gönderir

    public event Action<float> OnDamageUpgraded; // (New Damage Multiplier) gönderir
    public event Action<float> OnRangeUpgraded; // (New Range Multiplier) gönderir
    public event Action<float> OnSpeedUpgraded; // (New Speed Multiplier) gönderir
    public event Action<float> OnMaxHealthUpgraded; // (New Max Health Multiplier) gönderir
    public event Action<float> OnHealthRegenUpgraded; // (New Health Regen Multiplier) gönderir
    public event Action<float> OnLifeStealUpgraded; // (New LifeSteal Percentage) gönderir
    public event Action<float> OnAttackSpeedUpgraded; // (New Attack Speed Multiplier) gönderir
    public event Action<float> OnCriticalHitChanceUpgraded; // (New Critical Hit Chance) gönderir
    public event Action<float> OnCriticalHitDamageUpgraded; // (New Critical Hit Damage Multiplier) gönderir

    // --- Flags ---
    private bool isCriticalHit = false;

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
    public float GiveDamageCharacter()
    {
        float rawDamage;

        // Damage hesabı
        if (isCriticalHit)
        {
            isCriticalHit = false;
            rawDamage = Mathf.RoundToInt(CurrentDamage * DamageMultiplier * CriticalHitDamageMultiplier);
        }
        else
            rawDamage = CurrentDamage * DamageMultiplier;

        // Randomize
        float fluctuation = baseStats.DamageRangePercentage;
        float multiplier = (UnityEngine.Random.Range(-fluctuation, fluctuation) + 100) / 100;

        return Mathf.RoundToInt(rawDamage * multiplier);
    }

    // --- Upgrade Management Methods ---
    public void ApplyUpgrades(float damageMultiplier, float rangeMultiplier, float cooldownPercentage, float speedMultiplier, float healthMultiplier)
    {
        DamageMultiplier *= damageMultiplier;
        RangeMultiplier *= rangeMultiplier;
        AttackSpeedPercentage += cooldownPercentage;
        SpeedMultiplier *= speedMultiplier;
        MaxHealthBonus *= healthMultiplier;

        CurrentDamage = CurrentDamage * damageMultiplier;
        CurrentRange = CurrentRange * rangeMultiplier;
        CurrentSpeed = CurrentSpeed * speedMultiplier;
        MaxHealth = MaxHealth * MaxHealthBonus;
    }

    public void IncreaseDamageUpgrade(float amount)
    {
        DamageMultiplier += amount;
        OnDamageUpgraded?.Invoke(DamageMultiplier);
    }

    public void IncreaseRangeUpgrade(float amount)
    {
        RangeMultiplier += amount;
        OnRangeUpgraded?.Invoke(RangeMultiplier);
    }

    public void IncreaseSpeedUpgrade(float amount)
    {
        SpeedMultiplier += amount;
        OnSpeedUpgraded?.Invoke(SpeedMultiplier);
    }

    public void IncreaseMaxHPUpgrade(float amount)
    {
        MaxHealthBonus += amount;
        MaxHealth = baseStats.BaseMaxHealth + MaxHealthBonus;
        CurrentHealth += amount;
        OnMaxHealthUpgraded?.Invoke(MaxHealthBonus);
    }

    public void IncreaseHPRegenUpgrade(float amount)
    {
        HealthRegenBonus += amount;
        OnHealthRegenUpgraded?.Invoke(HealthRegenBonus);
    }

    public void IncreaseLifeStealUpgrade(float amount)
    {
        LifeStealPercentage += amount;
        OnLifeStealUpgraded?.Invoke(LifeStealPercentage);
    }

    public void IncreaseAttackSpeedUpgrade(float amount)
    {
        AttackSpeedPercentage += amount;
        OnAttackSpeedUpgraded?.Invoke(AttackSpeedPercentage);
    }

    public void IncreaseCriticalHitChanceUpgrade(float amount)
    {
        CriticalHitChance += amount;
        OnCriticalHitChanceUpgraded?.Invoke(CriticalHitChance);
    }

    public void IncreaseCriticalHitDamageUpgrade(float amount)
    {
        CriticalHitDamageMultiplier += amount;
        OnCriticalHitDamageUpgraded?.Invoke(CriticalHitDamageMultiplier);
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