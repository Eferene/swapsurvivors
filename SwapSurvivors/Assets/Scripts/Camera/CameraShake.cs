using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private PlayerManager playerManager;
    private Vector3 originalPosition;
    private float shakeTimer;
    private float shakeIntensity;
    private float lastShakeTime;

    [Header("Shake Settings")]
    [SerializeField] private bool shakeEnabled = true;
    [SerializeField] private float shakeCooldown = 0.2f;
    [SerializeField] private float maxShakeIntensity = 0.15f;
    [SerializeField] private float shakeDuration = 0.1f;

    private void Start()
    {
        originalPosition = transform.localPosition;

        // Player'ı bul ve event'e abone ol
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerManager = playerObj.GetComponent<PlayerManager>();
            playerManager.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDestroy()
    {
        // Memory leak önlemek için unsubscribe
        if (playerManager != null)
        {
            playerManager.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth, float damage)
    {
        if (Time.time - lastShakeTime < shakeCooldown) return;

        // Hasar miktarına göre intensity
        float damagePercent = damage / maxHealth;
        float intensity = Mathf.Clamp(damagePercent * 0.5f, 0.05f, maxShakeIntensity);

        Shake(intensity, shakeDuration);
        lastShakeTime = Time.time;
    }

    private void LateUpdate()
    {
        if (!shakeEnabled) return;

        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitCircle * shakeIntensity;
            transform.localPosition = originalPosition + shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }
    }

    private void Shake(float intensity, float duration)
    {
        if (!shakeEnabled) return;
        shakeIntensity = Mathf.Min(intensity, maxShakeIntensity);
        shakeTimer = duration;
    }

    public void SetShakeEnabled(bool enabled)
    {
        shakeEnabled = enabled;
        if (!enabled)
        {
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }
    }
}