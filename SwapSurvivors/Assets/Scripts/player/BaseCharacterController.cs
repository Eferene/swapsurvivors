using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseCharacterController : MonoBehaviour
{
    // --- References ---
    protected PlayerManager playerManager;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private InputActions controls;

    // --- Movement ---
    private Vector2 moveInput;
    private float lastStepTime = 0f;

    // --- Combat ---
    private float lastAttackTime = 0f;
    
    // --- Interactions ---
    protected Transform currentObject;

    // --- Unity Methods ---
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = GetComponent<PlayerManager>();

        controls = new InputActions();
        controls.Player.Move.performed += ctx => { moveInput = ctx.ReadValue<Vector2>(); };
        controls.Player.Move.canceled += ctx => { moveInput = Vector2.zero; };
        controls.Player.Interaction.performed += OnInteract;
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
        float currentSpeed = playerManager.CurrentSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, moveInput.y * currentSpeed);

        if (rb.linearVelocity != Vector2.zero && Time.time >= lastStepTime + 0.2f)
        {
            //AudioManager.Instance.PlayPlayerStepSFX();
            lastStepTime = Time.time;
        }
    }

    protected virtual void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentObject != null)
        {
            if(currentObject.TryGetComponent<Shop>(out Shop currentShop))
            {
                if (currentShop.playerInside)
                {
                    currentShop.OpenAndCloseShop();
                }
            }
            else if(currentObject.TryGetComponent<NewWaveArea>(out NewWaveArea newWaveArea))
            {
                if (newWaveArea.playerInside)
                {
                    newWaveArea.NewWave();
                }
            }
        }
    }

    public void SetCurrentObject(Transform obj) => currentObject = obj;
    public void ClearCurrentObject() => currentObject = null;

    // --- Abstract Methods ---
    protected abstract void Attack(); // Saldırı yöntemi, türetilmiş sınıflarda uygulanacak

    protected abstract float GetCooldown(); // Saldırı uygulama yöntemi, türetilmiş sınıflarda uygulanacak
}
