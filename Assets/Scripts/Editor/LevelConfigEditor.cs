using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelConfigEditor : EditorWindow
{
    private string csvFilePath => Path.Combine(Application.dataPath, "Scripts", "Day5", "level_config.csv");
    private List<LevelData> levels = new List<LevelData>();
    private Vector2 scrollPosition;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelConfigEditor>("Level Editor");
    }

    private void OnEnable()
    {
        LoadConfig();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Configuration", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load CSV", GUILayout.Height(30)))
        {
            LoadConfig();
        }
        if (GUILayout.Button("Save CSV", GUILayout.Height(30)))
        {
            SaveConfig();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Header
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label("Level", GUILayout.Width(40));
        GUILayout.Label("Scene Name", GUILayout.Width(150));
        GUILayout.Label("Spawns", GUILayout.Width(50));
        GUILayout.Label("Monster Prefabs (; separated)", GUILayout.ExpandWidth(true));
        GUILayout.Label("Actions", GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        // Rows
        for (int i = 0; i < levels.Count; i++)
        {
            var data = levels[i];
            EditorGUILayout.BeginHorizontal();

            data.level = EditorGUILayout.IntField(data.level, GUILayout.Width(40));
            data.sceneName = EditorGUILayout.TextField(data.sceneName, GUILayout.Width(150));
            data.spawnCount = EditorGUILayout.IntField(data.spawnCount, GUILayout.Width(50));

            // Convert List to string and back
            string prefabsString = data.monsterPrefabNames != null ? string.Join(";", data.monsterPrefabNames) : "";
            
            // Check if prefabs exist in Resources
            bool allPrefabsValid = ValidatePrefabs(data.monsterPrefabNames);
            
            // If invalid, tint the text field red to warn the user
            GUI.color = allPrefabsValid ? Color.white : new Color(1f, 0.4f, 0.4f); 

            string newPrefabsString = EditorGUILayout.TextField(prefabsString, GUILayout.ExpandWidth(true));
            if (newPrefabsString != prefabsString)
            {
                data.monsterPrefabNames = newPrefabsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                          .Select(p => p.Trim()).ToList();
            }

            GUI.color = Color.white; // Reset color

            if (GUILayout.Button("X", GUILayout.Width(60)))
            {
                levels.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Add New Level", GUILayout.Height(30)))
        {
            int nextLevel = levels.Count > 0 ? levels.Max(l => l.level) + 1 : 1;
            levels.Add(new LevelData() 
            { 
                level = nextLevel, 
                sceneName = "Level_Scene_Name", 
                spawnCount = 5, 
                monsterPrefabNames = new List<string> { "Enemy" } 
            });
        }
    }

    private void LoadConfig()
    {
        levels.Clear();
        if (!File.Exists(csvFilePath))
        {
            Debug.LogWarning($"[Level Editor] CSV not found at {csvFilePath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvFilePath);
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
                    if (!string.IsNullOrWhiteSpace(p))
                        data.monsterPrefabNames.Add(p.Trim());
                }

                levels.Add(data);
            }
        }
    }

    private void SaveConfig()
    {
        if (levels == null) return;

        List<string> lines = new List<string>();
        // Header
        lines.Add("Level,SceneName,SpawnCount,MonsterPrefabNames");

        foreach (var data in levels)
        {
            string prefabs = data.monsterPrefabNames != null ? string.Join(";", data.monsterPrefabNames) : "";
            lines.Add($"{data.level},{data.sceneName},{data.spawnCount},{prefabs}");
        }

        File.WriteAllLines(csvFilePath, lines);
        AssetDatabase.Refresh(); // Tells Unity to reload assets so ConfigLoader.cs reads the new data immediately
        Debug.Log($"<color=green><b>[Level Editor]</b></color> Saved successfully to CSV!");
    }

    private bool ValidatePrefabs(List<string> prefabNames)
    {
        if (prefabNames == null || prefabNames.Count == 0) return false;

        foreach (string p in prefabNames)
        {
            if (string.IsNullOrWhiteSpace(p)) continue;

            // Thêm đường dẫn "Prefabs/Enemy/" giống hệt cách WaveManager load quái vật
            GameObject loaded = Resources.Load<GameObject>($"Prefabs/Enemy/{p}");
            if (loaded == null)
            {
                return false; 
            }
        }
        return true;
    }
}
