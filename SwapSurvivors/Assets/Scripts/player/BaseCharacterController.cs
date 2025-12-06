using System.Collections;
using UnityEngine;

public abstract class BaseCharacterController : MonoBehaviour
{
    // --- Components ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private InputActions controls;

    // --- Movement ---
    private Vector2 moveInput;
    protected float playerSpeed;
    private float lastStepTime = 0f;

    // --- Combat ---
    private float lastAttackTime = 0f;

    // --- Unity Methods ---
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controls = new InputActions();

        controls.Player.Move.performed += ctx => { moveInput = ctx.ReadValue<Vector2>(); };
        controls.Player.Move.canceled += ctx => { moveInput = Vector2.zero; };
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    protected virtual void Update()
    {
        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;

        ApplyAttack();
    }

    protected virtual void ApplyAttack()
    {
        // Saldırı cooldown kontrolü
        if (Time.time >= lastAttackTime + GetCooldown())
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * playerSpeed, moveInput.y * playerSpeed);

        if(rb.linearVelocity != Vector2.zero && Time.time >= lastStepTime + 0.2f)
        {
            AudioManager.Instance.PlayPlayerStepSFX();
            lastStepTime = Time.time;
        }
    }

    // --- Abstract Methods ---
    protected abstract void Attack(); // Saldırı yöntemi, türetilmiş sınıflarda uygulanacak

    protected abstract float GetCooldown(); // Saldırı uygulama yöntemi, türetilmiş sınıflarda uygulanacak
}
