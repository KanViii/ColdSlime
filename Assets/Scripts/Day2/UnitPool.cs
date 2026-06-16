using UnityEngine;
using System.Collections.Generic;


public class UnitPool : MonoBehaviour
{
    [SerializeField] private int initialPoolSize = 10;

    Dictionary<BaseUnitConfig, Queue<GameObject>> pools = new();
    Dictionary<BaseUnitConfig, GameObject> prefabs = new();

    private static UnitPool _instance;

    public static UnitPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<UnitPool>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(UnitPool).ToString());
                    _instance = singleton.AddComponent<UnitPool>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }


    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    void Start()
    {
        InitializePools(EnemyManager.Instance.EnemyConfigs);
    }

    void InitializePools(List<BaseUnitConfig> configs)
    {
        for (int i = 0; i < configs.Count; i++)
        {
            for (int j = 0; j < initialPoolSize; j++)
            {
                AddToPool(configs[i]);
            }
        }
    }

    void AddToPool(BaseUnitConfig config)
    {
        if (!pools.ContainsKey(config))
        {
            pools[config] = new();
        }

        GameObject prefab;
        if (!prefabs.ContainsKey(config))
        {
            prefab = Resources.Load<GameObject>($"Prefabs/{config.Name}");
            prefabs[config] = prefab;
        }
        else prefab = prefabs[config];


        GameObject instance = Instantiate(prefab);
        instance.SetActive(false);
        pools[config].Enqueue(instance);
    }

    public GameObject GetUnit(BaseUnitConfig config)
    {
        if (!pools.ContainsKey(config) || pools[config].Count <= 0)
        {
            AddToPool(config);
        }

        GameObject instance = pools[config].Dequeue();
        instance.SetActive(true);
        return instance;
    }

    public void ReleaseUnit(BaseUnitController unit)
    {
        if (unit != null)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
            pools[unit.Config].Enqueue(unit.transform.GetChild(0).gameObject);
        }
    }
}
