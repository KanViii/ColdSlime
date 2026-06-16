using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<> list;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            
        }
    }

    public void SpawnEnemy()
    {
        BaseUnitConfig baseUnitConfig = _enemyConfigs[Random.]
    }
}
