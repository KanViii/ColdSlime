using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public WaveDataSO waveData;       
    public GameObject enemyPrefab;   
    public int poolSize = 10;         

    private List<GameObject> enemyPool = new List<GameObject>();
    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isSpawningFinished = false;
    private Coroutine spawnCoroutine;
    private bool isGameOver = false;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); 
            enemyPool.Add(obj);
        }

        Enemy.OnAnyEnemyDied += HandleEnemyDeath;
        Player.OnPlayerDied += HandleGameOver;
        spawnCoroutine = StartCoroutine(SpawnWaveRoutine());
    }

    void HandleGameOver()
    {
        isGameOver = true;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    void Update()
    {
        if (isGameOver) return;

            // Debug.Log("Wave Update");
        for (int i=0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] !=  null) 
            {
                activeEnemies[i].CustomUpdate();
            }
        }
    }


    GameObject GetEnemyFromPool()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)  return enemy;
        }
        return null; 
    }

    IEnumerator SpawnWaveRoutine()
    {
        Debug.Log("--- WAVE STARTED ! ---");
        isSpawningFinished = false;

        foreach (UnitStatsSO enemyStats in waveData.enemiesToSpawn)
        {
            yield return new WaitForSeconds(Random.Range(waveData.minspawnInterval, waveData.maxspawInterval));

            GameObject enemy = GetEnemyFromPool();
            if (enemy != null)
            {
                enemy.transform.position = Player.Instance.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                enemy.SetActive(true); 
                
                Enemy enemyScripts = enemy.GetComponent<Enemy>();
                if (enemyScripts != null && !activeEnemies.Contains(enemyScripts))
                    activeEnemies.Add(enemyScripts);
            }
        }
        
        isSpawningFinished = true;
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        Debug.Log($"An enemy was died! Remaining in active: {activeEnemies.Count}");

        if (isSpawningFinished && activeEnemies.Count <= 0)
        {
            Debug.Log("--- YOU WIN THIS WAVE ! ---");
        }
    }

    void OnDestroy()
    {
        Enemy.OnAnyEnemyDied -= HandleEnemyDeath;
        Player.OnPlayerDied -= HandleGameOver;
    }
}