using UnityEngine;
using System.Collections.Generic;

namespace Day8
{
    [System.Serializable]
    public class MonsterStats
    {
        public int level;
        public float maxHealth;
        public float moveSpeed;
        public float attackDamage;
        public float attackRange;
        public float attackCooldown;
        public int maxMonsters;
    }

    [CreateAssetMenu(fileName = "NewMonsterLevelData", menuName = "Day8/Monster Level Data")]
    public class MonsterLevelData : ScriptableObject
    {
        public List<MonsterStats> levels = new List<MonsterStats>();

        public MonsterStats GetStatsForLevel(int level)
        {
            foreach (var stat in levels)
            {
                if (stat.level == level)
                {
                    return stat;
                }
            }
            
            // Return a default stat if level not found, or the first one
            if (levels.Count > 0)
            {
                Debug.LogWarning($"[MonsterLevelData] Stats for level {level} not found. Returning default stats.");
                return levels[0];
            }

            Debug.LogError("[MonsterLevelData] No stats configured in ScriptableObject!");
            return new MonsterStats() { level = level, maxHealth = 10f, moveSpeed = 3f, attackDamage = 1f, attackRange = 1.5f, attackCooldown = 1f, maxMonsters = 10 };
        }
    }
}
