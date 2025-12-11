using UnityEngine;
using System.Collections;
using System;

public class SuicideAttack : EnemyAttack
{
    float startTime;
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        if (Vector2.Distance(enemyTransform.position + enemyTransform.GetComponent<EnemyController>().enemyData.attackOffset, targetTransform.position) <= range) // Menzil kontrolü
        {
            int dmg = Convert.ToInt32(UnityEngine.Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f))); // Hasar aralığını hesapla

            PlayerManager playerManager = targetTransform.GetComponent<PlayerManager>();
            startTime = Time.time;

            while(Time.time - startTime < 0.5f)
            {
                Debug.Log("test");
            }

            playerManager.TakeDamageCharacter(dmg);

            return true;
        }
        return false;
    }
}
