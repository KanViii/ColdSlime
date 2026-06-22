using UnityEngine;
using System.Collections.Generic;
using System.IO;

public enum ConfigSourceType
{
    CSV,
    JSON,
    ScriptableObject
}

public static class ConfigLoader
{
    public static List<LevelData> LoadConfig(ConfigSourceType sourceType, string configDir, LevelConfigSO scriptableObjectRef)
    {
        switch (sourceType)
        {
            case ConfigSourceType.ScriptableObject:
                return LoadFromScriptableObject(scriptableObjectRef);
        }
        return null;
    }

    public static List<LevelData> LoadFromScriptableObject(LevelConfigSO asset)
    {
        if (asset == null)
        {
            Debug.LogError("[ConfigLoader] ScriptableObject reference is null.");
            return new List<LevelData>();
        }
        Debug.Log($"[ConfigLoader] Successfully loaded {asset.levels.Count} levels from ScriptableObject.");
        return asset.levels;
    }

    public static List<LevelData> LoadFromCsv(string fullPath)
    {
        List<LevelData> levels = new List<LevelData>();

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[ConfigLoader] Không tìm thấy file CSV tại đường dẫn: {fullPath}");
            return levels;
        }

        string[] lines = File.ReadAllLines(fullPath);
        for (int i = 1; i < lines.Length; i++) 
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length >= 4)
            {
                LevelData data = new LevelData();
                int.TryParse(columns[0], out data.level);
                data.sceneName = columns[1].Trim();
                int.TryParse(columns[2], out data.spawnCount);
                
                string[] prefabs = columns[3].Split(';'); 
                data.monsterPrefabNames = new List<string>();
                foreach (string p in prefabs)
                {
                    data.monsterPrefabNames.Add(p.Trim());
                }

                levels.Add(data);
            }
        }
        return levels;
    }
}
