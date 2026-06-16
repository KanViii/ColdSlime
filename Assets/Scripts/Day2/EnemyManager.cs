using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<BaseUnitConfig> _enemyConfigs;
    [SerializeField] private BaseUnitController _enemyHolder;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _minSpawnInterval = 2f;
    [SerializeField] private float _maxSpawnInterval = 5f;


    public List<BaseUnitController> Enemies => _enemies;
    public List<BaseUnitConfig> EnemyConfigs => _enemyConfigs;

    private List<BaseUnitController> _enemies = new List<BaseUnitController>();

    private float _spawnTimer;
    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // _instance = FindAnyObjectType<EnemyManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(EnemyManager).ToString());
                    _instance = singleton.AddComponent<EnemyManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _enemies = new List<BaseUnitController>();
    }

    void Start()
    {
        SpawnEnemy();
    }


    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0) SpawnEnemy();
        
        for (int i = _enemies.Count - 1; i>= 0; i--)
        {
            if (_enemies[i] == null) _enemies.RemoveAt(i);
        }

        for (int i = 0; i < Enemies.Count; i++) _enemies[i].CustomUpdate();
    }

    public void SpawnEnemy()
    {
        BaseUnitConfig baseUnitConfig = _enemyConfigs[Random.Range(0, _enemyConfigs.Count)];
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        BaseUnitController enemyHolder = Instantiate(_enemyHolder, spawnPoint.position, Quaternion.identity);
        enemyHolder.Initialize(baseUnitConfig);

        GameObject enemy = UnitPool.Instance.GetUnit(baseUnitConfig);

        enemy.transform.SetParent(enemyHolder.transform);

        _enemies.Add(enemyHolder);

        _spawnTimer = Random.Range(_minSpawnInterval, _maxSpawnInterval);
    }
}
