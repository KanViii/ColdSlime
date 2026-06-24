using UnityEngine;
using System.Collections;

namespace Day8
{
    public class MonsterManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string monsterPrefabPath = "Prefabs/Monster/BlueSlime";
        [SerializeField] private MonsterLevelData levelData;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private float spawnInterval = 3f;

        public MonsterLevelData LevelData => levelData;
        public int CurrentLevel => currentLevel;
        // [SerializeField] private float spawnRadius = 10f;

        private MonsterPool pool;
        private bool isSpawning = false;
        private System.Collections.Generic.List<Monster> activeMonsters = new System.Collections.Generic.List<Monster>();
        private int spawnedCountInCurrentLevel = 0;
        private bool isWaitingForNextLevel = false;

        void Start()
        {
            InitializeSystem();
        }

        void Update()
        {
            for (int i = activeMonsters.Count - 1; i >= 0; i--)
            {
                if (activeMonsters[i] != null && activeMonsters[i].gameObject.activeInHierarchy)
                {
                    activeMonsters[i].CustomUpdate();
                }
                else
                {
                    activeMonsters.RemoveAt(i);
                }
            }

            CheckLevelCleared();
        }

        private void CheckLevelCleared()
        {
            if (isWaitingForNextLevel || levelData == null) return;

            MonsterStats currentStats = levelData.GetStatsForLevel(currentLevel);
            if (spawnedCountInCurrentLevel >= currentStats.maxMonsters && activeMonsters.Count == 0)
            {
                StartCoroutine(LevelClearedRoutine());
            }
        }

        private IEnumerator LevelClearedRoutine()
        {
            isWaitingForNextLevel = true;

            if (Player.Instance != null)
            {
                Player.Instance.PlayWinSound();
            }

            Debug.Log($"[MonsterManager] Level {currentLevel} cleared! Progressing to next level...");

            // Get score from PlayManager if it exists
            int currentScore = 0;
            PlayManager playManager = Object.FindFirstObjectByType<PlayManager>();
            if (playManager != null)
            {
                currentScore = playManager.CurrentScore;
            }

            // Save Progress (score is 0 for the new level)
            SaveSystem.SaveToJson(currentLevel + 1, 0, 0);

            yield return new WaitForSeconds(1.5f); // Wait a bit before next level

            SetLevel(currentLevel + 1);
            isWaitingForNextLevel = false;
        }

        private void InitializeSystem()
        {
            // Load saved level first
            GameSaveData data = SaveSystem.LoadFromJson(1, 0, 0);
            currentLevel = data.currentLevel;
            spawnedCountInCurrentLevel = 0;
            isWaitingForNextLevel = false;

            // 1. Dynamically Load Prefab
            GameObject prefabObj = Resources.Load<GameObject>(monsterPrefabPath);
            if (prefabObj == null)
            {
                Debug.LogError($"[MonsterManager] Failed to load prefab at Resources/{monsterPrefabPath}. Make sure it is in a Resources folder.");
                return;
            }

            Monster monsterPrefab = prefabObj.GetComponent<Monster>();
            if (monsterPrefab == null)
            {
                Debug.LogError("[MonsterManager] Loaded prefab does not have a Monster script attached!");
                return;
            }

            // 2. Setup Pool
            GameObject poolObj = new GameObject("MonsterPool");
            poolObj.transform.SetParent(this.transform);
            
            pool = poolObj.AddComponent<MonsterPool>();
            pool.Initialize(monsterPrefab);

            // 3. Start Spawning
            if (levelData == null)
            {
                Debug.LogWarning("[MonsterManager] MonsterLevelData is not assigned. Spawning will not start or will use default stats.");
            }
            
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (isSpawning)
            {
                yield return new WaitForSeconds(spawnInterval);
                
                MonsterStats stats = levelData != null 
                    ? levelData.GetStatsForLevel(currentLevel) 
                    : new MonsterStats() { level = currentLevel, maxMonsters = 10 };

                if (spawnedCountInCurrentLevel < stats.maxMonsters)
                {
                    SpawnMonster();
                }
            }
        }

        private void SpawnMonster()
        {
            if (Player.Instance == null) return;

            // Find Spawnpoint if not assigned
            GameObject spawnPointObj = GameObject.Find("Spawnpoint");
            if (spawnPointObj == null)
            {
                Debug.LogWarning("[MonsterManager] GameObject named 'Spawnpoint' not found in the scene! Cannot spawn monster.");
                return;
            }

            Vector3 spawnPos = spawnPointObj.transform.position;

            // Get from pool
            Monster monster = pool.Get();
            monster.transform.position = spawnPos;

            // Initialize stats based on level
            MonsterStats stats = levelData != null 
                ? levelData.GetStatsForLevel(currentLevel) 
                : new MonsterStats() { level = currentLevel, maxHealth = 10, moveSpeed = 3, attackDamage = 1, attackRange = 1.5f, attackCooldown = 1f, maxMonsters = 10 };

            monster.Initialize(stats);
            activeMonsters.Add(monster);
            spawnedCountInCurrentLevel++;
        }
        
        public void SetLevel(int level)
        {
            if (currentLevel != level)
            {
                currentLevel = level;
                spawnedCountInCurrentLevel = 0; // Reset counter for new level
                Debug.Log($"[MonsterManager] Level set to {currentLevel}");

                // Update UI instantly
                PlayManager playManager = Object.FindFirstObjectByType<PlayManager>();
                if (playManager != null) 
                {
                    playManager.ResetScore();
                    playManager.UpdateLevelUI();
                }
            }
        }
    }
}
