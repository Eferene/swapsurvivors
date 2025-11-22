using UnityEngine;

public class MeleeAttack : BaseAttackType
{
    public override void Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        if(Vector3.Distance(enemyTransform.position, targetTransform.position) <= range) // Menzil kontrolü
        {
            float dmg = Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f)); // Hasar aralığını hesapla
            dmg = Mathf.Round(dmg * 10f) / 10f; // Ondalık hassasiyetini ayarlamak için

            PlayerStats.Instance.DecreaseHealth(dmg);
            Debug.Log("| " + dmg + " | Player hit! Current Health: " + PlayerStats.Instance.PlayerHealth);
        }
    }
}