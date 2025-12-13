using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [Header("Pool Settings")]
    public GameObject[] enemyPrefabs;
    public int poolSizePerEnemy = 50;
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        foreach (var prefab in enemyPrefabs)
        {
            for (int i = 0; i < poolSizePerEnemy; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = prefab.name;
                obj.transform.parent = this.transform;
                obj.SetActive(false);
                if(!poolDictionary.ContainsKey(prefab.name)) poolDictionary.Add(prefab.name, new Queue<GameObject>());
                poolDictionary[prefab.name].Enqueue(obj);
            }
        }
    }

    public GameObject GetEnemyFromPool(GameObject prefab)
    {
        string key = prefab.name;

        if(!poolDictionary.ContainsKey(key)) poolDictionary.Add(key, new Queue<GameObject>());
        
        if(poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = this.transform;
            obj.name = prefab.name;
            return obj;
        }
    }

    public void ReturnEnemyToPool(GameObject obj)
    {
        obj.SetActive(false);
        string key = obj.name;

        if (poolDictionary.ContainsKey(key))
        {
            poolDictionary[key].Enqueue(obj);
        }
        else
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            newQueue.Enqueue(obj);
            poolDictionary.Add(key, newQueue);
        }
    }

    public void ReturnAllEnemiesToPool()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.gameObject.activeInHierarchy)
            {
                EnemyController enemyController = child.transform.GetComponent<EnemyController>();
                if(enemyController != null) enemyController.DieEffect();

                ReturnEnemyToPool(child);
            }
        }
    }
}
