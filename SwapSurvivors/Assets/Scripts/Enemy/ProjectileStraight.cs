using UnityEngine;

public class ProjectileStraight : ProjectileBase
{
    public float damage = 10f;
    private Transform target;
    private bool tracked = false;
    private Vector2 direction;

    protected override void Start()
    {
        base.Start();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
            direction = (target.position - transform.position).normalized;
            tracked = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void Move()
    {
        if (tracked)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    protected override void OnHitTarget(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats.Instance.DecreaseHealth(damage);
            Destroy(gameObject);
        }
    }
}
