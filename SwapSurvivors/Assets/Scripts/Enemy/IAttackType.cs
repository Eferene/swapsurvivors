using UnityEngine;

public interface IAttackType
{
    void Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range);
}

public abstract class BaseAttackType : MonoBehaviour, IAttackType
{
    public abstract void Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range);
}