using UnityEngine;

public class ProjectilePredictive : ProjectileBase
{
    public float damage = 10f;
    private Transform target;
    private bool tracked = false;
    private Vector2 direction;
    private Vector2 currentVelocity;

    protected override void Start()
    {
        base.Start();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
            Vector2 playerVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

            Vector2 toPlayer = (Vector2)playerObj.transform.position - (Vector2)transform.position;

            // Lead shot hesaplaması: solve (v_p^2 - s^2) t^2 + 2 (toPlayer . v_p) t + toPlayer^2 = 0
            float a = playerVelocity.sqrMagnitude - speed * speed;
            float b = 2f * Vector2.Dot(toPlayer, playerVelocity);
            float c = toPlayer.sqrMagnitude;

            Vector2 aimDirection = toPlayer.normalized; // default aim directly at current position

            float t = -1f;

            const float eps = 1e-6f;
            if (Mathf.Abs(a) < eps)
            {
                // Degenerate to linear: b * t + c = 0 => t = -c / b
                if (Mathf.Abs(b) > eps)
                {
                    float tLinear = -c / b;
                    if (tLinear > 0f) t = tLinear;
                }
            }
            else
            {
                float discriminant = b * b - 4f * a * c;
                if (discriminant >= 0f)
                {
                    float sqrtD = Mathf.Sqrt(discriminant);
                    float t1 = (-b + sqrtD) / (2f * a);
                    float t2 = (-b - sqrtD) / (2f * a);

                    // choose smallest positive time
                    float tPos = float.MaxValue;
                    if (t1 > 0f && t1 < tPos) tPos = t1;
                    if (t2 > 0f && t2 < tPos) tPos = t2;
                    if (tPos != float.MaxValue) t = tPos;
                }
            }

            if (t > 0f)
            {
                aimDirection = (toPlayer + playerVelocity * t).normalized;
            }

            currentVelocity = aimDirection * speed; // velocity already includes speed
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
            // currentVelocity already includes speed (aimDirection * speed)
            transform.Translate(currentVelocity * Time.deltaTime);
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
