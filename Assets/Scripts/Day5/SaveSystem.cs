using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public int currentLevel;
    public int score;
    public int enemiesKilled;
}

public static class SaveSystem
{
    private static readonly string PlayerPrefsLevelKey = "SavedLevel";
    private static string JsonFilePath => Path.Combine(Application.persistentDataPath, "gamesave.json");

    // Save to PlayerPrefs
    public static void SaveToPlayerPrefs(int level, int score, int enemiesKilled)
    {
        PlayerPrefs.SetInt(PlayerPrefsLevelKey, level);
        PlayerPrefs.SetInt("SavedScore", score);
        PlayerPrefs.SetInt("SavedEnemiesKilled", enemiesKilled);
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] Saved Level {level} to PlayerPrefs.");
    }

    // Load from PlayerPrefs
    public static GameSaveData LoadFromPlayerPrefs(int defaultLevel = 1, int defaultScore = 0, int defaultEnemies = 0)
    {
        if (PlayerPrefs.HasKey(PlayerPrefsLevelKey))
        {
            int level = PlayerPrefs.GetInt(PlayerPrefsLevelKey);
            int score = PlayerPrefs.GetInt("SavedScore", defaultScore);
            int enemies = PlayerPrefs.GetInt("SavedEnemiesKilled", defaultEnemies);
            Debug.Log($"[SaveSystem] Loaded Level {level}, Score {score}, Enemies {enemies} from PlayerPrefs.");
            return new GameSaveData { currentLevel = level, score = score, enemiesKilled = enemies };
        }
        Debug.Log($"[SaveSystem] No save found in PlayerPrefs. Returning default.");
        return new GameSaveData { currentLevel = defaultLevel, score = defaultScore, enemiesKilled = defaultEnemies };
    }

    // Save to custom JSON file
    public static void SaveToJson(int level, int score, int enemiesKilled)
    {
        try
        {
            GameSaveData data = new GameSaveData { currentLevel = level, score = score, enemiesKilled = enemiesKilled };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(JsonFilePath, json);
            Debug.Log($"[SaveSystem] Saved Level {level} to JSON file: {JsonFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save JSON: {e.Message}");
        }
    }

    // Load from custom JSON file
    public static GameSaveData LoadFromJson(int defaultLevel = 1, int defaultScore = 0, int defaultEnemies = 0)
    {
        try
        {
            string path = JsonFilePath;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log($"[SaveSystem] Loaded Level {data.currentLevel}, Score {data.score}, Enemies {data.enemiesKilled} from JSON file.");
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load JSON: {e.Message}");
        }
        Debug.Log($"[SaveSystem] No JSON save file found. Returning default.");
        return new GameSaveData { currentLevel = defaultLevel, score = defaultScore, enemiesKilled = defaultEnemies };
    }

    // Check if save exists
    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(PlayerPrefsLevelKey) || File.Exists(JsonFilePath);
    }

    // Clear saves
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsLevelKey);
        PlayerPrefs.DeleteKey("SavedScore");
        PlayerPrefs.DeleteKey("SavedEnemiesKilled");
        PlayerPrefs.Save();
        if (File.Exists(JsonFilePath))
        {
            File.Delete(JsonFilePath);
        }
        Debug.Log("[SaveSystem] Cleared all save data.");
    }
}
