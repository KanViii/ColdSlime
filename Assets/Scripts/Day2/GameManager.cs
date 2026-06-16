using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BaseUnitConfig _playerConfig;
    [SerializeField] private PlayerController _playerHolder;
    private BaseUnitController _player;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
             {
                // _instance = FindAnyObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    _instance = singleton.AddComponent<GameManager>();
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

        PlayerController playerHolder = Instantiate(_playerHolder, Vector3.zero, Quaternion.identity);
        playerHolder.Initialize(_playerConfig);

        GameObject playerPrefab = Resources.Load<GameObject>($"Prefabs/{_playerConfig.Name}");
        GameObject playerObject = Instantiate(playerPrefab);
        playerObject.transform.SetParent(playerHolder.transform);

        playerObject.GetComponent<Renderer>().material.color = Color.blue;

        _player = playerHolder;
    }

    void Update()
    {
        if (_player != null)
        {
            _player.CustomUpdate();
        }
    }
    
    public BaseUnitController FindEnemy()
    {
        if (EnemyManager.Instance != null && EnemyManager.Instance.Enemies.Count > 0)
            return EnemyManager.Instance.Enemies[0];
        return null;
    }
        
}
