using UnityEngine;
using System.Collections;
using System;

public class RaycastAttack : EnemyAttack
{
    public override bool Attack(Transform enemyTransform, Transform targetTransform, float damage, float damagePercentage, float range)
    {
        int mask = LayerMask.GetMask("Player");

        Vector2 origin = enemyTransform.position + enemyTransform.GetComponent<EnemyController>().enemyData.attackOffset;
        Vector2 dir = (targetTransform.position - enemyTransform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, mask);

        if (hit.collider != null)
        {
            int dmg = Convert.ToInt32(UnityEngine.Random.Range(damage * (1 - damagePercentage / 100f), damage * (1 + damagePercentage / 100f))); // Hasar aralığını hesapla

            StartCoroutine(FireLaser(enemyTransform.GetComponent<EnemyController>(), origin, hit.point, dir, 0.2f, 0.3f, dmg));
            return true;
        }
        return false;
    }

    IEnumerator FireLaser(EnemyController enemy, Vector2 origin, Vector2 hitPos, Vector2 direction, float expandTime, float laserDuration, float damage)
    {
        enemy.isAttacking = true;
        Vector2 laserEndPoint = hitPos + direction * 100f;

        LineRenderer core = CreateLR("CoreLaser", 0f, 0f, Color.red);
        core.SetPosition(0, origin);
        core.SetPosition(1, laserEndPoint);

        // Animasyon
        float t = 0f;
        float startW = 0f;
        float coreW = 0.125f;

        while (t < expandTime)
        {
            t += Time.deltaTime;
            float f = t / expandTime;

            core.startWidth = Mathf.Lerp(startW, coreW, f);
            core.endWidth = core.startWidth;

            yield return null;
        }

        yield return new WaitForSeconds(laserDuration);
    
        Destroy(core.gameObject);

        int mask = LayerMask.GetMask("Player");
        RaycastHit2D finalHit = Physics2D.Raycast(origin, direction, Vector2.Distance(origin, laserEndPoint), mask);
    
        if (finalHit.collider != null && finalHit.collider.CompareTag("Player"))
        {
            PlayerStats.Instance.DecreaseHealth(damage);
        }
        
        enemy.isAttacking = false;
    }

    LineRenderer CreateLR(string name, float startW, float endW, Color col)
    {
        LineRenderer lr = new GameObject(name).AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = startW;
        lr.endWidth = endW;
        lr.startColor = col;
        lr.endColor = col;
        lr.positionCount = 2;
        lr.gameObject.transform.parent = this.gameObject.transform;
        return lr;
    }
}
