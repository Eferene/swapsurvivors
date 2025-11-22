using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI progressText;

    void Start()
    {
        StartCoroutine(LoadAsyncOperation(SceneData.sceneToLoad));
    }

    IEnumerator LoadAsyncOperation(int sceneID)
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneID);

        while (!gameLevel.isDone)
        {
            float progress = Mathf.Clamp01(gameLevel.progress / 0.9f);
            slider.value = progress;
            progressText.text = "%" + (progress * 100f).ToString("F2");
            yield return null;
        }
    }
}
