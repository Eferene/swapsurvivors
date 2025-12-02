using System;
using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        if(Vector2.Distance(enemyTransform.position + enemyTransform.GetComponent<EnemyController>().enemyData.attackOffset, targetTransform.position) <= range) // Menzil kontrolü
        {
            int dmg = Convert.ToInt32(UnityEngine.Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f))); // Hasar aralığını hesapla

            PlayerStats.Instance.DecreaseHealth(dmg);
            Debug.Log("| " + dmg + " | Player hit! Current Health: " + PlayerStats.Instance.PlayerHealth);
            return true;
        }
        return false;
    }
}