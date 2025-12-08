using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangCharacter : BaseCharacterController
{
    [Header("Boomerang Stats")]
    [SerializeField] private float boomerangSpeed = 15.0f;
    [SerializeField] private float detectionRadius = 10.0f;

    [Header("Components")]
    [SerializeField] private Transform ThrowPoint;
    [SerializeField] private GameObject BoomerangPrefab;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private List<GameObject> boomerangPool = new List<GameObject>(); // Bumerang havuzu

    protected override float GetCooldown() => playerManager.CurrentCooldown;

    protected override void ApplyAttack()
    {
        base.ApplyAttack();
        FindClosestEnemy();
    }

    protected override void Attack()
    {
        switch (playerManager.CharacterLevel)
        {
            case 1: LevelOneAttack(); break;
            case 2: LevelTwoAttack(); break;
            case 3: LevelThreeAttack(); break;
            default: LevelOneAttack(); break;
        }
    }
    private void LevelOneAttack()
    {
        ThrowBoomerang();
    }
    private void LevelTwoAttack()
    {
    }
    private void LevelThreeAttack()
    {
    }
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

    private void FindClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;
    }

    private void ThrowBoomerang()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - ThrowPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject boomerang = GetBoomerangFromPool(rotation);
        boomerang.transform.position = ThrowPoint.position;

        if (boomerang.TryGetComponent(out BoomerangController boomerangScript))
            boomerang.GetComponent<BoomerangController>().Setup(playerManager.GiveDamageCharacter(), playerManager.CurrentRange, boomerangSpeed, this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
