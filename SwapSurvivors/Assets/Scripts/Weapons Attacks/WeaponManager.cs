using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // --- ORTAK �ZELL?KLER (Her karakterde olacak) ---
    [Header("Can Sistemi")]
    public float maxCan = 100f;
    private float mevcutCan;

    [Header("Hasar Sistemi")]
    public float hasarKatsayisi = 1.0f; // Al?nan hasara �arpan

    // --- �ZEL �ZELL?KLER (Sadece Sava?�?da olacak) ---
    [Header("Sava?�? �zellikleri")]
    public float hareketHizi = 5f;
    public float kilicHasari = 25f;
    public float menzil = 2f;

    // Bile?enler
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Bile?enleri al
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Can? max yap
        mevcutCan = maxCan;

        Debug.Log("Sava?�? oyuna haz?r!");
    }

    void Update()
    {
        // Hareket et
        float yatayInput = Input.GetAxisRaw("Horizontal"); // A-D veya Ok tu?lar?
        rb.linearVelocity = new Vector2(yatayInput * hareketHizi, rb.linearVelocity.y);

        // Karakteri hareket y�n�ne �evir
        if (yatayInput > 0)
            transform.localScale = new Vector3(1, 1, 1); // Sa?a bak
        else if (yatayInput < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak

        // Sol mouse ile sald?r
        if (Input.GetMouseButtonDown(0))
        {
            Saldir();
        }

        // TEST: K tu?u ile kendine hasar
        if (Input.GetKeyDown(KeyCode.K))
        {
            HasarAl(20f);
        }
    }

    // Sald?r? yap
    void Saldir()
    {
        Debug.Log("Sava?�? k?l?� sallad?!");

        // Basit sald?r?: Yak?ndaki d�?manlar? bul
        Collider2D[] dusmanllar = Physics2D.OverlapCircleAll(
            transform.position,
            menzil
        );

        foreach (var dusman in dusmanllar)
        {
            // Kendine vurmayal?m
            if (dusman.gameObject == this.gameObject)
                continue;

            Debug.Log($"{dusman.name} k?l?�tan {kilicHasari} hasar ald?!");
        }
    }

    // Hasar al
    public void HasarAl(float hasar)
    {
        float gercekHasar = hasar * hasarKatsayisi;
        mevcutCan -= gercekHasar;

        Debug.Log($"Sava?�? {gercekHasar} hasar ald?! Kalan can: {mevcutCan}");

        // K?rm?z? yan?p s�ns�n
        spriteRenderer.color = Color.red;
        Invoke("RengiDuzenle", 0.1f);

        // �ld� m�?
        if (mevcutCan <= 0)
        {
            Debug.Log("Sava?�? �ld�!");
            Destroy(gameObject);
        }
    }

    void RengiDuzenle()
    {
        spriteRenderer.color = Color.white;
    }

    // Sald?r? menzilini g�ster (Editor'de)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, menzil);
    }
}