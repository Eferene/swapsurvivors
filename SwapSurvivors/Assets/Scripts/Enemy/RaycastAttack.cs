using UnityEngine;

public class RaycastAttack : BaseAttackType
{
    public override void Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        int mask = LayerMask.GetMask("Player");

        Vector2 origin = enemyTransform.position;
        Vector2 dir = (targetTransform.position - enemyTransform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, mask);

        Debug.DrawLine(origin, origin + dir * range, Color.green, 0.1f);

        if (hit.collider != null)
        {
            float dmg = Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f));
            dmg = Mathf.Round(dmg * 10f) / 10f;

            LineRenderer lr = new GameObject("RaycastLine").AddComponent<LineRenderer>();

            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.positionCount = 2;
            lr.SetPosition(0, origin);
            lr.SetPosition(1, targetTransform.position);
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            Destroy(lr.gameObject, 0.15f);

            PlayerStats.Instance.DecreaseHealth(dmg);
        }
    }
}
