using UnityEngine;

public class ScytheOnTrigger : MonoBehaviour
{
    ScyhteCharacter scytheCharacter;
    private void Awake()
    {
        scytheCharacter = GetComponentInParent<ScyhteCharacter>();
        transform.localScale = new Vector3(scytheCharacter.ScytheRange / 8, scytheCharacter.ScytheRange / 4, 1f);
        transform.localPosition = new Vector3(0f, transform.localScale.y + 0.5f, 0f);
        Debug.Log(transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyController enemyController))
            {
                float damage = PlayerStats.Instance.GiveDamage(scytheCharacter.ScytheDamage);
                enemyController.TakeDamage(damage);
                Debug.Log($"{collision.name} gelen {damage} hasarı yedi.");
            }
        }
    }
}
