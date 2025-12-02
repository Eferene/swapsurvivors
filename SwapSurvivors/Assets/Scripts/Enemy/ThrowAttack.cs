using System.Collections.Generic;
using UnityEngine;

public class ThrowAttack : EnemyAttack
{
    List <Vector3> selectedPoints = new List<Vector3>();
    List <GameObject> instantiatedThrows = new List<GameObject>();
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        // Temizleme
        selectedPoints.Clear();
        instantiatedThrows.Clear();

        // Saldırı
        if(Vector2.Distance(enemyTransform.position + enemyTransform.GetComponent<EnemyController>().enemyData.attackOffset, targetTransform.position) <= range) // Menzil kontrolü
        {
            EnemyController enemyController = enemyTransform.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                for(int i = 0; i < enemyController.enemyData.throwCount; i++)
                {
                    Vector3 randomPoint = targetTransform.position + Random.insideUnitSphere * enemyController.enemyData.throwRadius;
                    randomPoint.z = -1;
                    selectedPoints.Add(randomPoint);

                    GameObject newThrowPrefab = Instantiate(enemyController.enemyData.throwPrefab, randomPoint, Quaternion.identity);
                    ThrowObject throwObject = newThrowPrefab.GetComponent<ThrowObject>();
                    throwObject.enemyData = enemyController.enemyData;
                    instantiatedThrows.Add(newThrowPrefab);
                }
                return true;
            }
        }
        return false;
    }
}
