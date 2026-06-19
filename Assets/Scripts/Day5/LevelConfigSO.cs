using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfigSO")]
public class LevelConfigSO : ScriptableObject
{
    public List<LevelData> levels;
}
