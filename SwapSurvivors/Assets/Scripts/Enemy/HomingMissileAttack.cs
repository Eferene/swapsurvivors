using System;
using UnityEngine;

public class HomingMissileAttack : EnemyAttack
{
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        if(Vector2.Distance(enemyTransform.position + enemyTransform.GetComponent<EnemyController>().enemyData.attackOffset, targetTransform.position) <= range) // Menzil kontrolü
        {
            EnemyController enemyController = enemyTransform.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.enemyData.projectilePrefab != null)
            {
                GameObject projectileGameObject = Instantiate(enemyController.enemyData.projectilePrefab, enemyTransform.position, Quaternion.identity);

                int dmg = Convert.ToInt32(UnityEngine.Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f))); // Hasar aralığını hesapla

                ProjectileHoming projectile = projectileGameObject.GetComponent<ProjectileHoming>();
                projectile.damage = dmg;
                projectile.speed = enemyController.enemyData.projectileSpeed;

                return true;
            }
        }
        return false;
    }
}
