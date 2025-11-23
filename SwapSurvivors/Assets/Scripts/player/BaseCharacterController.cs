using UnityEngine;

public abstract class BaseCharacterController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected InputActions controls;
    protected Vector2 moveInput;
    protected EnemyController enemyController;

    [SerializeField] protected float playerSpeed;
    [SerializeField] protected float attackCooldown = 1.0f;

    protected float lastAttackTime = 0f;

    protected virtual void Awake()
    {
        playerSpeed = PlayerStats.Instance.PlayerSpeed;
        rb = GetComponent<Rigidbody2D>();
        controls = new InputActions();

        controls.Player.Move.performed += ctx => { moveInput = ctx.ReadValue<Vector2>(); };
        controls.Player.Move.canceled += ctx => { moveInput = Vector2.zero; };
    }

    protected virtual void OnEnable() => controls.Player.Enable();
    protected virtual void OnDisable() => controls.Player.Disable();

    protected virtual void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * playerSpeed, moveInput.y * playerSpeed);
    }

    protected abstract void Attack();
}
