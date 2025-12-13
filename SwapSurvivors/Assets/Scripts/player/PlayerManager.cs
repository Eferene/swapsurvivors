using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables
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
    public int CharacterLevel { get; private set; } = 2;
    public int Score { get; private set; }

    // --- Upgrade Stats --- Kod üzerinde çarpan olarak kullanılır (x1.2 = +20%)
    public float DamagePercentage { get; private set; }
    public float RangePercentage { get; private set; }
    public float SpeedPercentage { get; private set; }
    public float AttackSpeedPercentage { get; private set; }

    public float MaxHealthBonus { get; private set; }
    public float HealthRegenBonus { get; private set; }
    public float LifeStealPercentage { get; private set; }
    public float ArmorPercentage { get; private set; }

    public float CriticalHitChance { get; private set; }
    public float CriticalHitDamageMultiplier { get; private set; }

    private const float MAX_LIFE_STEAL = 5f;
    private const float MAX_HP_REGEN = 11f;
    private const float SOFT_CAP_SCALE = 200f;

    // --- Events (UI) ---
    public event Action<float, float, float> OnHealthChanged; // (Current, Max, Damage) gönderir
    public event Action<int> OnScoreChanged; // (New Score) gönderir
    public event Action<int> OnLevelChanged; // (New Level) gönderir
    public event Action OnPlayerDied;
    public event Action<bool> OnDamageHitOccurred;
    public event Action OnStatsUpdated;

    // --- Flags ---
    #endregion

    private void Awake() => InitializeStats();

    private void InitializeStats()
    {
        // SO'dan verileri çekip runtime değişkenlere yazıyoruz
        // Base attack stats
        MaxHealth = baseStats.BaseMaxHealth;
        CurrentHealth = MaxHealth;
        CurrentDamage = baseStats.BaseDamage;
        CurrentSpeed = baseStats.BaseSpeed;
        CurrentCooldown = baseStats.BaseAttackCooldown;
        CurrentRange = baseStats.BaseRange;

        // Base upgrade stats
        DamagePercentage = baseStats.InitialDamageMultiplier;
        RangePercentage = baseStats.InitialRangeMultiplier;
        SpeedPercentage = baseStats.InitialSpeedMultiplier;
        AttackSpeedPercentage = baseStats.InitialAttackSpeedMultiplier;

        CriticalHitChance = baseStats.InitialCritChance;
        CriticalHitDamageMultiplier = baseStats.InitialCritDamageMultiplier;
        ArmorPercentage = baseStats.InitialArmorPercentage;
        LifeStealPercentage = baseStats.InitialLifeStealRate;
        HealthRegenBonus = baseStats.InitialHealthRegen;
        MaxHealthBonus = 0f;

        Debug.Log("PlayerManager: Statlar yüklendi");
    }

    #region --- Health Management Methods ---
    public void TakeDamageCharacter(float amount)
    {
        float takenDamage = amount * (1 - ArmorPercentage / 100f);

        CurrentHealth -= takenDamage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        // UI'a haber
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth, amount);

        if (CurrentHealth <= 0)
        {
            Debug.Log("Die");
            OnPlayerDied?.Invoke();
        }
    }

    public void HealCharacter(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth, amount);
    }
    #endregion

    #region --- Damage Management Methods ---
    public float CalculateDamage()
    {
        // Damage hesabı
        float damage = CurrentDamage * DamagePercentage;

        // Randomize
        float fluctuation = baseStats.DamageRangePercentage;
        float randomFactor = (UnityEngine.Random.Range(-fluctuation, fluctuation) + 100) / 100;
        damage *= randomFactor;

        if (UnityEngine.Random.value <= CriticalHitChance)
        {
            damage *= CriticalHitDamageMultiplier;
            OnDamageHitOccurred?.Invoke(true);
        }
        else
            OnDamageHitOccurred?.Invoke(false);

        damage = Mathf.RoundToInt(damage);

        return damage;
    }

    public void ApplyOnHitEffects(float damageDealt)
    {
        float lifeStealRate = ApplySoftCap(LifeStealPercentage, MAX_LIFE_STEAL, SOFT_CAP_SCALE) / 100;

        if (lifeStealRate > 0)
        {
            float healAmount = Mathf.RoundToInt(damageDealt * lifeStealRate);
            if (healAmount > 0)
            {
                HealCharacter(healAmount);
                Debug.Log($"Lifesteal çalıştı: {healAmount} can geldi.");
            }
        }
    }
    #endregion

    #region --- Upgrade Management Methods ---
    public void IncreaseDamageUpgrade(float amount)
    {
        DamagePercentage += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseRangeUpgrade(float amount)
    {
        RangePercentage += amount;
        CurrentRange = baseStats.BaseRange * RangePercentage;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseSpeedUpgrade(float amount)
    {
        SpeedPercentage += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseMaxHPUpgrade(float amount)
    {
        MaxHealthBonus += amount;
        UpdateMaxHealth();
        CurrentHealth += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseHPRegenUpgrade(float amount)
    {
        HealthRegenBonus += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseLifeStealUpgrade(float amount)
    {
        LifeStealPercentage += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseAttackSpeedUpgrade(float amount)
    {
        AttackSpeedPercentage += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseCriticalHitChanceUpgrade(float amount)
    {
        CriticalHitChance += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseCriticalHitDamageUpgrade(float amount)
    {
        CriticalHitDamageMultiplier += amount;
        OnStatsUpdated?.Invoke();
    }

    public void IncreaseArmor(float amount)
    {
        ArmorPercentage += amount;
    }

    private void UpdateMaxHealth()
    {
        MaxHealth = baseStats.BaseMaxHealth + MaxHealthBonus;
    }
    #endregion

    #region --- Score and Level Management Methods ---
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
    #endregion

    private static float ApplySoftCap(float stat, float max, float speed)
    {
        // Logaritmik azalan getiri formülü
        float A = max; // Maksimum sağlık yenileme hızı
        float B = speed; // Azalan getiri hızı
        return A * (1f - Mathf.Exp(-stat / B));
    }
}