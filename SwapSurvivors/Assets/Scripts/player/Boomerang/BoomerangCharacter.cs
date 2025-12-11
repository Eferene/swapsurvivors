using System.Collections.Generic;
using UnityEngine;

public class BoomerangCharacter : BaseCharacterController
{
    #region Variables

    [Header("Boomerang Stats")]
    [SerializeField] private float boomerangSpeed = 15.0f;
    [SerializeField] private float detectionRadius = 10.0f;

    [Header("Components")]
    [SerializeField] private Transform ThrowPoint;
    [SerializeField] private GameObject BoomerangPrefab;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private List<GameObject> boomerangPool = new List<GameObject>(); // Bumerang havuzu

    #endregion

    #region Base Overrides
    protected override float GetCooldown() => playerManager.CurrentCooldown;

    protected override void ApplyAttack()
    {
        base.ApplyAttack();
        FindClosestEnemy();
    }

    protected override void Attack() => ThrowBoomerang();

    #endregion

    #region Combat Logic
    private void ThrowBoomerang()
    {
        if (currentTarget == null) return;

        // Yön ve rotasyon hesabı
        Vector3 direction = (currentTarget.position - ThrowPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Havuzdan çekme
        GameObject boomerang = GetBoomerangFromPool(rotation);
        boomerang.transform.position = ThrowPoint.position;

        // Setup boomerang
        if (boomerang.TryGetComponent(out BoomerangController boomerangScript))
            boomerang.GetComponent<BoomerangController>().Setup(playerManager, playerManager.CurrentRange, boomerangSpeed,
                playerManager.CharacterLevel, this.gameObject);
    }

    private void FindClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemy in enemiesInRange)
        {
            // Kendimize çok yakın ölü bir enemy'i hedef almayalım
            if (enemy.TryGetComponent(out EnemyController ec) && ec.IsDead) continue;

            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;
    }
    #endregion

    #region Object Pooling
    private GameObject GetBoomerangFromPool(Quaternion rotation)
    {
        foreach (GameObject obj in boomerangPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newBoomerang = Instantiate(BoomerangPrefab);
        boomerangPool.Add(newBoomerang);
        return newBoomerang;
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    #endregion
}
