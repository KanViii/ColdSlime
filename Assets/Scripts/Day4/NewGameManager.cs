using UnityEngine;

public class NewGameManager : MonoBehaviour
{
    public static NewGameManager Instance { get; private set; }

    public GameObject uiRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(this.gameObject);

        if (uiRoot != null)
        {
            uiRoot.transform.SetParent(null);
            DontDestroyOnLoad(uiRoot);
        }
    }

    private void Start()
    {
        // Phải gọi hàm Init() thì code bên trong mới chạy
        Init();
    }

    private void Init()
    {
        if (CanvasManager.Instance == null)
        {
            Debug.LogError("CanvasManager.Instance đang null! Hãy đảm bảo CanvasManager đã có trong Scene.");
            return;
        }

        // Đã bỏ đoạn code gọi UIMainMenu ở đây vì LevelManager.cs đã đảm nhận việc gọi UI
        // Dựa vào tên scene (Main -> gọi MainMenu, ugly_enemy -> gọi HUD)
    }
}
