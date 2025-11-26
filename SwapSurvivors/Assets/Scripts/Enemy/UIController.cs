using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
}