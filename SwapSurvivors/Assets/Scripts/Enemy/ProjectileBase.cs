using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        Move();
    }   

    protected abstract void Move();
    protected abstract void OnHitTarget(Collider2D collision);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHitTarget(collision); 
    }
}   