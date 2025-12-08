using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image healthImage;
    [SerializeField] TextMeshProUGUI healthText;

    private PlayerManager playerManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject playerobj = GameObject.FindWithTag("Player");
            playerManager = playerobj.GetComponent<PlayerManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        playerManager.OnHealthChanged += UpdateHealthSlider;
        playerManager.OnScoreChanged += UpdateScoreText;
    }

    private void OnDisable()
    {
        playerManager.OnHealthChanged -= UpdateHealthSlider;
        playerManager.OnScoreChanged -= UpdateScoreText;
    }

    private void Start()
    {
        UpdateScoreText(playerManager.Score);
        UpdateHealthSlider(playerManager.CurrentHealth, playerManager.MaxHealth);
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateHealthSlider(float maxHealt, float currentHealt)
    {
        healthText.text = playerManager.CurrentHealth + "/" + playerManager.MaxHealth;
        healthImage.fillAmount = playerManager.CurrentHealth / playerManager.MaxHealth;
    }
}