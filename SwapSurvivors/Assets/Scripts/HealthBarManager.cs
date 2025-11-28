using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    RectTransform healthBarTransform;
    Rigidbody2D playerRb;
    [SerializeField] private Image healthBarImage;

    [SerializeField] private float yOffset = 1.5f;

    void Start()
    {
        healthBarTransform = GetComponent<RectTransform>();
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float playerMaxHealth = PlayerStats.Instance.PlayerMaxHealth;
        float playerHealth = PlayerStats.Instance.PlayerHealth;

        healthBarImage.fillAmount = playerHealth / playerMaxHealth;
    }

    private void LateUpdate()
    {
        if (playerRb == null) playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        healthBarTransform.transform.position =
            new Vector3(playerRb.transform.position.x, playerRb.transform.position.y + yOffset, playerRb.transform.position.z);
    }
}
