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
        if (enemyPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(enemyPrefab);
                obj.name = enemyPrefab.name;
                obj.SetActive(false); 
                enemyPool.Add(obj);
            }
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


    GameObject GetEnemy(string prefabName)
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy && enemy.name.StartsWith(prefabName))  
                return enemy;
        }

        // Nếu pool hết hoặc không có loại này trong pool, tự động load và tạo thêm
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Enemy/{prefabName}");
        if (prefab != null)
        {
            GameObject newObj = Instantiate(prefab);
            newObj.name = prefabName;
            newObj.SetActive(false);
            enemyPool.Add(newObj);
            return newObj;
        }

        return null; 
    }

    IEnumerator SpawnWaveRoutine()
    {
        Debug.Log("--- WAVE STARTED ! ---");
        isSpawningFinished = false;

        LevelData currentLevelData = null;
        if (LevelManager.Instance != null)
        {
            currentLevelData = LevelManager.Instance.GetCurrentLevelData();
        }

        int spawnCount = 0;
        List<string> prefabsToSpawn = new List<string>();

        if (currentLevelData != null)
        {
            spawnCount = currentLevelData.spawnCount;
            if (LevelManager.Instance != null)
            {
                int alreadyKilled = LevelManager.Instance.EnemiesKilledThisLevel;
                spawnCount = Mathf.Max(0, spawnCount - alreadyKilled);
            }
            prefabsToSpawn = currentLevelData.monsterPrefabNames;
        }
        else if (waveData != null)
        {
            spawnCount = waveData.enemiesToSpawn.Count;
            if (enemyPrefab != null) prefabsToSpawn.Add(enemyPrefab.name);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            float minInterval = waveData != null ? waveData.minspawnInterval : 1f;
            float maxInterval = waveData != null ? waveData.maxspawInterval : 3f;
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            string chosenPrefabName = "Enemy"; 
            if (prefabsToSpawn != null && prefabsToSpawn.Count > 0)
            {
                chosenPrefabName = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
            }

            GameObject enemy = GetEnemy(chosenPrefabName);
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

        if (spawnCount <= 0)
        {
            HandleEnemyDeath(null);
        }
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        Debug.Log($"An enemy was died! Remaining in active: {activeEnemies.Count}");

        if (isSpawningFinished && activeEnemies.Count <= 0)
        {
            Debug.Log("--- YOU WIN THIS WAVE ! ---");
            if (Player.Instance != null && Player.Instance.audioWin != null)
            {
                // Dùng PlayClipAtPoint để phát nhạc chiến thắng
                AudioSource.PlayClipAtPoint(Player.Instance.audioWin, Player.Instance.transform.position);
            }

            // Tự động chuyển qua level tiếp theo
            if (LevelManager.Instance != null)
            {
                int nextLevel = LevelManager.Instance.CurrentLevel + 1;
                LevelData nextLevelData = LevelManager.Instance.LevelConfigs.Find(l => l.level == nextLevel);
                
                if (nextLevelData != null)
                {
                    Debug.Log($"Tiến hành Auto Load Level {nextLevel}...");
                    LevelManager.Instance.SetLevel(nextLevel);
                    
                    // Lấy tên scene hiện tại
                    string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    
                    // Nếu sceneName của level tiếp theo khác với scene hiện hành -> Load Scene mới
                    if (!string.IsNullOrEmpty(nextLevelData.sceneName) && nextLevelData.sceneName != currentScene)
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelData.sceneName);
                    }
                    else
                    {
                        // Vẫn ở chung một scene -> Chạy tiếp đợt quái tiếp theo luôn
                        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
                        spawnCoroutine = StartCoroutine(SpawnWaveRoutine());
                    }
                }
                else
                {
                    Debug.Log("Chúc mừng! Bạn đã hoàn thành tất cả các level trong config!");
                }
            }
            else
            {
                // Fallback nếu không có LevelManager (vòng lặp vô tận WaveManager cũ)
                if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
                spawnCoroutine = StartCoroutine(SpawnWaveRoutine());
            }
        }
    }

    void OnDestroy()
    {
        Enemy.OnAnyEnemyDied -= HandleEnemyDeath;
        Player.OnPlayerDied -= HandleGameOver;
    }
}