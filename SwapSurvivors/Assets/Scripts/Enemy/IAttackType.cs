using UnityEngine;

public interface IAttackType
{
    bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range);
}

public abstract class BaseAttackType : MonoBehaviour, IAttackType
{ 
    public abstract bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range);
}