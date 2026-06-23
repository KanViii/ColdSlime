using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static event System.Action<int> OnLevelChanged;
    public static event System.Action<int> OnScoreChanged;

    [Header("Config Settings")]
    [Tooltip("Đường dẫn tương đối từ thư mục Assets tới file CSV")]
    public string csvFilePath = "Scripts/Day5/level_config.csv";

    public List<LevelData> LevelConfigs { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentScore { get; private set; }
    public int EnemiesKilledThisLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGameData();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.ClearAllUI();
        }

        string currentScene = scene.name;
        
        if (currentScene == "Main")
        {
            MainMenuManager uiMenu = CanvasManager.Instance.LoadPrefabs<MainMenuManager>("UIMainMenu"); 
            CanvasManager.Instance.AddUI(uiMenu);
        }
        else
        {
            PlayManager uiPlay = CanvasManager.Instance.LoadPrefabs<PlayManager>("UIManHUD"); 
            CanvasManager.Instance.AddUI(uiPlay);
        }
    }

    public void LoadGameData()
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, csvFilePath);
        LevelConfigs = ConfigLoader.LoadFromCsv(fullPath);
        if (LevelConfigs != null)
        {
            Debug.Log($"[LevelManager] Loaded {LevelConfigs.Count} level configs.");
        }
        else
        {
            Debug.LogWarning("[LevelManager] Failed to load level configs.");
        }

        GameSaveData data = SaveSystem.LoadFromJson(1, 0, 0);
        CurrentLevel = data.currentLevel;
        CurrentScore = data.score;
        EnemiesKilledThisLevel = data.enemiesKilled;
        Debug.Log($"[LevelManager] Game Loaded. Current Level: {CurrentLevel}, Score: {CurrentScore}, EnemiesKilledThisLevel: {EnemiesKilledThisLevel}");
    }

    public void SaveGameData()
    {
        SaveSystem.SaveToJson(CurrentLevel, CurrentScore, EnemiesKilledThisLevel);
        Debug.Log("[LevelManager] Game Saved.");
    }

    public void SetLevel(int level)
    {
        CurrentLevel = level;
        EnemiesKilledThisLevel = 0; // Reset số quái khi qua level mới
        OnLevelChanged?.Invoke(CurrentLevel);
        SaveGameData();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        EnemiesKilledThisLevel += amount;
        OnScoreChanged?.Invoke(CurrentScore);
        SaveGameData();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
        SaveGameData();
    }
    
    public LevelData GetCurrentLevelData()
    {
        if (LevelConfigs == null) return null;
        return LevelConfigs.Find(l => l.level == CurrentLevel);
    }
}
