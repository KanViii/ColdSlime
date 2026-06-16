using UnityEngine;

public class UnitPool : MonoBehaviour
{
    [SerializeField] private int initialPoolSize = 10;

    Dictionary<BaseUnitConfig, Queue<GameObject>> pools = new();
    Dictionary<BaseUnitConfig, GameObject> prefabs = new();

    private static 


    void Awake()
    {
        if (_instance)
    }


    void Start()
    {
        initialPools(EnemyManager.Instance.EnemyConfigs);
    }

    void InitializePools(List<BaseUnitConfig> configs)
    {
        for (int i = 0; i < configs.Count; i++)
        {
            for (int j=0; j < initialPoolSize; j++)
            {
                AddToPool(pools[configs[i]]);
            }
        }
    }

    void AddToPool(BaseUnitConfig config)
    {
        if (!pools.ContainsKey(configs[i]))
        {
            pools[configs[i]] = new();
        }

        GameObject instance = Resource.Load<GameObject>($"Prefab/{config.Name}");
        GameObject instance = Instantiate(prefab);
        instance.SetActive(fasle);
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
        if (unit == null)
        {
            unit.transform.GetChild(0).gameObject.SecActive(fasle);
            pools[unit.Config].Enqueue(unit.transform.GetChild(0).gameObject);
        }
    }
}
