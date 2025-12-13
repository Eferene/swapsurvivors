using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class WaveController : MonoBehaviour
{
    public static WaveController Instance;
    private UIController uiController;

    [Header("Main Wave Settings")]
    public List<WaveConfig> waves = new List<WaveConfig>();

    [Header("General Settings")]
    public int radius;
    public TextMeshProUGUI waveText;
    private int currentWave = 0;
    private bool isEndlessMode = false;
    public bool isBreakdown;

    [Header("Breakdown Elements")]
    public Transform breakdownGameObjects;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            uiController = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIController>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NewWave();
    }

    public void NewWave()
    {
        StopShopPhase();
        isBreakdown = false;
        WaveConfig cWave = waves[currentWave];
        waveText.text = "Wave " + (currentWave + 1);

        if(!isEndlessMode)
        {
            StartCoroutine(ExecuteMainWave(cWave));
        }
        currentWave++;
    }

    private IEnumerator ExecuteMainWave(WaveConfig wave)
    {
        StartCoroutine(uiController.StartWaveTimer(wave.waveDurationSec));

        foreach(var enemyPattern in wave.enemiesToSpawn)
        {
            StartCoroutine(ExecuteEnemyPattern(enemyPattern, wave.waveDurationSec));
        }

        yield return new WaitForSeconds(wave.waveDurationSec + 0.1f);

        EnemyPool.Instance.ReturnAllEnemiesToPool();
        StartShopPhase();
    }

    private IEnumerator ExecuteEnemyPattern(EnemyPattern enemyPattern, float waveDuration)
    {
        float timer = 0f;

        while(timer < waveDuration)
        {
            yield return new WaitForSeconds(enemyPattern.spawningDurationSec);

            foreach(var enemyPatternObject in enemyPattern.enemySequence)
            {
                for(int i = 0; i < enemyPatternObject.quantity; i++)
                {
                    GameObject newEnemy = EnemyPool.Instance.GetEnemyFromPool(enemyPatternObject.enemyData.enemyPrefab);

                    Vector3 randomPoint = GetRandomPoint();
                    while (IsVisibleByCamera(randomPoint)) randomPoint = GetRandomPoint();

                    newEnemy.transform.position = randomPoint;
                }
            }

            timer += enemyPattern.spawningDurationSec;
        }
    }

    private void StartShopPhase()
    {
        isBreakdown = true;
        waveText.text = "Breakdown";
        for(int i = 0; i < breakdownGameObjects.childCount; i++)
        {
            breakdownGameObjects.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void StopShopPhase()
    {
        for(int i = 0; i < breakdownGameObjects.childCount; i++)
        {
            breakdownGameObjects.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private Vector3 GetRandomPoint()
    {
        float r = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * radius;
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2f);
        return new Vector3(
            Mathf.Cos(angle) * r,
            Mathf.Sin(angle) * r,
            -1
        );
    }

    private bool IsVisibleByCamera(Vector2 point)
    {
        Vector2 view = Camera.main.WorldToViewportPoint(point);
        return view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1;
    }
}