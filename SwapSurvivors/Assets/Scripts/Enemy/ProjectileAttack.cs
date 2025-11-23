using UnityEngine;

public class ProjectileAttack : BaseAttackType
{
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        if(Vector3.Distance(enemyTransform.position, targetTransform.position) <= range) // Menzil kontrolü
        {
            EnemyController enemyController = enemyTransform.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.enemyData.projectilePrefab != null)
            {
                GameObject projectileGameObject = Instantiate(enemyController.enemyData.projectilePrefab, enemyTransform.position, Quaternion.identity);

                float dmg = Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f)); // Hasar aralığını hesapla
                dmg = Mathf.Round(dmg * 10f) / 10f; // Ondalık hassasiyetini ayarlamak için

                ProjectileStraight projectile = projectileGameObject.GetComponent<ProjectileStraight>();
                projectile.damage = dmg;
                projectile.speed = enemyController.enemyData.projectileSpeed;

                return true;
            }
        }
        return false;
    }
}
